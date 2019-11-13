using UnityEditor;
using UnityEngine;
using Sebastian.Geometry;


[CustomEditor(typeof(PolygonCreator))]
public class PolygonsEditor : Editor
{
    PolygonCreator polygonCreator;
    SelectionInfo selectionInfo;
    bool shapeChangedSinceLastRepaint;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        string helpMessage = "Left Click to add points.\nShift-Left Click on point to delete.\nShift-Left Click on empty space to create new shape";
        EditorGUILayout.HelpBox(helpMessage,MessageType.Info);

        int shapeDeleteIndex = -1;

        polygonCreator.showShapesList = EditorGUILayout.Foldout(polygonCreator.showShapesList, "Show Shapes List");

        if (polygonCreator.showShapesList)
        {

            for (int i = 0; i < polygonCreator.shapes.Count; i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label("Shape" + (i + 1));


                GUI.enabled = i != selectionInfo.selectedShapeIndex;
                if (GUILayout.Button("Select"))
                {
                    selectionInfo.selectedShapeIndex = i;
                }

                GUI.enabled = true;
                if (GUILayout.Button("Delete"))
                {
                    shapeDeleteIndex = i;
                }
                GUILayout.EndHorizontal();
            }
        }

        if (shapeDeleteIndex != -1)
        {
            Undo.RecordObject(polygonCreator, "Delete shape");
            polygonCreator.shapes.RemoveAt(shapeDeleteIndex);
            selectionInfo.selectedShapeIndex = Mathf.Clamp(selectionInfo.selectedShapeIndex, 0, polygonCreator.shapes.Count - 1);
        }

        if (GUI.changed)
        {
            shapeChangedSinceLastRepaint = true;
            SceneView.RepaintAll();
        }
    }

    private void OnSceneGUI()
    {
        Event guiEvent = Event.current;
        if (guiEvent.type == EventType.Repaint)
        {
            Draw();
        }
        else if (guiEvent.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        else
        {
            HandleInput(guiEvent);
            if (shapeChangedSinceLastRepaint)
            {
                HandleUtility.Repaint();

            }
        }

    }



    private void CreateNewShape()
    {
        Undo.RecordObject(polygonCreator, "Create Shape");
        polygonCreator.shapes.Add(new Shape());
        selectionInfo.selectedShapeIndex = polygonCreator.shapes.Count - 1;
    }


    private void HandleInput(Event guiE)
    {
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiE.mousePosition);

        float drawPlainHeight = 0;

        float distToDrawPlane = (drawPlainHeight - mouseRay.origin.y) / mouseRay.direction.y;

        //Vector3 mousePosition = mouseRay.origin + mouseRay.direction * distToDrawPlane;

        Vector3 mousePosition = mouseRay.GetPoint(distToDrawPlane);




        if (guiE.type == EventType.MouseDown && guiE.button == 0 && guiE.modifiers == EventModifiers.Shift)
        {
            HandleShiftLeftMouseDown(mousePosition);
        }

        if (guiE.type == EventType.MouseDown && guiE.button == 0 && guiE.modifiers == EventModifiers.None)
        {
            HandleLeftMouseButtoDown(mousePosition);
        }



        if (guiE.type == EventType.MouseUp && guiE.button == 0)
        {
            HandleLeftMouseButtonUP(mousePosition);
        }

        if (guiE.type == EventType.MouseDrag && guiE.button == 0 && guiE.modifiers == EventModifiers.None)
        {
            HandleLeftMouseButtonDrag(mousePosition);
        }


        if (!selectionInfo.pointIsSelected)
        {
            UpdateMouseOverInfo(mousePosition);
        }
    }

    private void OnEnable()
    {
        shapeChangedSinceLastRepaint = true;
        polygonCreator = target as PolygonCreator;
        selectionInfo = new SelectionInfo();
        Undo.undoRedoPerformed += OnUndoOrRedo;
        Tools.hidden = true;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoOrRedo;
        Tools.hidden = false;
    }
    void OnUndoOrRedo()
    {
        if (selectionInfo.selectedShapeIndex >= polygonCreator.shapes.Count || selectionInfo.selectedShapeIndex == -1)
        {
            selectionInfo.selectedShapeIndex = polygonCreator.shapes.Count - 1;
        }
        shapeChangedSinceLastRepaint = true;
    }

    void UpdateMouseOverInfo(Vector3 mousePos)
    {
        int mouseOverPointIndex = -1;

        int mouseOverShapeIndex = -1;

        for (int shapeIndex = 0; shapeIndex < polygonCreator.shapes.Count; shapeIndex++)
        {
            Shape currentShape = polygonCreator.shapes[shapeIndex];
            for (int i = 0; i < currentShape.points.Count; i++)
            {
                if (Vector3.Distance(mousePos, currentShape.points[i]) < polygonCreator.handleRadius)
                {
                    mouseOverPointIndex = i;
                    mouseOverShapeIndex = shapeIndex;
                    break;
                }
            }
        }


        if (mouseOverPointIndex != selectionInfo.pointIndex || mouseOverShapeIndex != selectionInfo.mouseOverShapeIndex)
        {
            selectionInfo.mouseOverShapeIndex = mouseOverShapeIndex;
            selectionInfo.pointIndex = mouseOverPointIndex;
            selectionInfo.mouseIsOvePoint = mouseOverPointIndex != -1;
            shapeChangedSinceLastRepaint = true;
        }

        if (selectionInfo.mouseIsOvePoint)
        {
            selectionInfo.mouseIsOnLine = false;
            selectionInfo.lineIndex = -1;
        }
        else
        {
            int mouseOverLineIndex = -1;

            float closestLineDist = polygonCreator.handleRadius;
            for (int shapeIndex = 0; shapeIndex < polygonCreator.shapes.Count; shapeIndex++)
            {
                Shape currentShape = polygonCreator.shapes[shapeIndex];
                for (int i = 0; i < currentShape.points.Count; i++)
                {
                    Vector3 nextPointInShape = currentShape.points[(i + 1) % currentShape.points.Count];

                    float distFromMouseToLine = HandleUtility.DistancePointToLineSegment(mousePos.ToXZ(), currentShape.points[i].ToXZ(), nextPointInShape.ToXZ());

                    if (distFromMouseToLine < closestLineDist)
                    {
                        closestLineDist = distFromMouseToLine;
                        mouseOverLineIndex = i;
                        mouseOverShapeIndex = shapeIndex;
                    }
                }
            }


            if (selectionInfo.lineIndex != mouseOverLineIndex || mouseOverShapeIndex != selectionInfo.mouseOverShapeIndex)
            {
                selectionInfo.mouseOverShapeIndex = mouseOverShapeIndex;
                selectionInfo.lineIndex = mouseOverLineIndex;
                selectionInfo.mouseIsOnLine = mouseOverLineIndex != -1;
                shapeChangedSinceLastRepaint = true;
            }
        }

    }

    void HandleLeftMouseButtoDown(Vector3 mousePos)
    {
        if (polygonCreator.shapes.Count == 0)
        {
            CreateNewShape();
        }

        SelectShapeUnderMouse();

        if (!selectionInfo.mouseIsOvePoint)
        {
            SelectPointUnderMouse();
        }
        else
        {
            CreateNewPoint(mousePos);
        }

        //selectionInfo.pointIsSelected = true;
        //selectionInfo.positionAtStartOfDrag = mousePos;
        //needsRepaint = true;
    }

    void HandleLeftMouseButtonUP(Vector3 mousePos)
    {
        if (selectionInfo.pointIsSelected)
        {

            SelectedShape.points[selectionInfo.pointIndex] = selectionInfo.positionAtStartOfDrag;
            Undo.RecordObject(polygonCreator, "MovePoint");

            SelectedShape.points[selectionInfo.pointIndex] = mousePos;

            selectionInfo.pointIsSelected = false;
            selectionInfo.pointIndex = -1;
            shapeChangedSinceLastRepaint = true;
        }
    }

    void HandleLeftMouseButtonDrag(Vector3 mousePos)
    {
        if (selectionInfo.pointIsSelected)
        {
            SelectedShape.points[selectionInfo.pointIndex] = mousePos;
            shapeChangedSinceLastRepaint = true;
        }
    }

    void HandleShiftLeftMouseDown(Vector3 mousePos)
    {
        if (selectionInfo.mouseIsOvePoint)
        {
            SelectShapeUnderMouse();
            DeletePointUnderMouse();

        }
        else
        {
            CreateNewShape();
            CreateNewPoint(mousePos);
        }

    }



    private void Draw()
    {

        for (int shapeIndex = 0; shapeIndex < polygonCreator.shapes.Count; shapeIndex++)
        {

            Shape shapeToDraw = polygonCreator.shapes[shapeIndex];
            bool shapeIsSelected = shapeIndex == selectionInfo.selectedShapeIndex;
            bool mouseIsOverShape = shapeIndex == selectionInfo.mouseOverShapeIndex;

            Color deselectedShapeColor = Color.grey;

            for (int i = 0; i < shapeToDraw.points.Count; i++)
            {
                Vector3 nextPoint = shapeToDraw.points[(i + 1) % shapeToDraw.points.Count];


                if (i == selectionInfo.lineIndex && mouseIsOverShape)
                {
                    Handles.color = Color.blue;
                    Handles.DrawLine(shapeToDraw.points[i], nextPoint);
                }
                else
                {
                    Handles.color = (shapeIsSelected) ? Color.white : deselectedShapeColor;
                    Handles.DrawDottedLine(shapeToDraw.points[i], nextPoint, 2);

                }



                if (i == selectionInfo.pointIndex && mouseIsOverShape)
                {
                    Handles.color = (selectionInfo.pointIsSelected) ? Color.black : Color.white;
                }
                else
                {
                    Handles.color = (shapeIsSelected) ? Color.blue : deselectedShapeColor;
                }
                Handles.DrawSolidDisc(shapeToDraw.points[i], Vector3.up, polygonCreator.handleRadius);
            }

            if (shapeChangedSinceLastRepaint)
            {
                polygonCreator.UpdateMeshDisplay();
            }

            shapeChangedSinceLastRepaint = false;
        }
    }

    void CreateNewPoint(Vector3 pos)
    {

        bool mouseIsOveSelectedShape = selectionInfo.mouseOverShapeIndex == selectionInfo.selectedShapeIndex;
        int newPointIndex = (selectionInfo.mouseIsOnLine && mouseIsOveSelectedShape) ? selectionInfo.lineIndex + 1 : SelectedShape.points.Count;

        Undo.RecordObject(polygonCreator, "AddPoint");
        SelectedShape.points.Insert(newPointIndex, pos);
        // Debug.Log("Add:" + mousePosition);
        selectionInfo.pointIndex = newPointIndex;
        selectionInfo.mouseOverShapeIndex = selectionInfo.selectedShapeIndex;
        shapeChangedSinceLastRepaint = true;



        SelectPointUnderMouse();
    }
    void DeletePointUnderMouse()
    {
        Undo.RecordObject(polygonCreator, "DeletePoint");
        SelectedShape.points.RemoveAt(selectionInfo.pointIndex);
        selectionInfo.pointIsSelected = false;
        selectionInfo.mouseIsOvePoint = false;
        shapeChangedSinceLastRepaint = true;
    }

    void SelectPointUnderMouse()
    {
        selectionInfo.pointIsSelected = true;
        selectionInfo.mouseIsOvePoint = true;
        selectionInfo.mouseIsOnLine = false;
        selectionInfo.lineIndex = -1;

        selectionInfo.positionAtStartOfDrag = SelectedShape.points[selectionInfo.pointIndex];
        shapeChangedSinceLastRepaint = true;
    }

    void SelectShapeUnderMouse()
    {
        if (selectionInfo.mouseOverShapeIndex != -1)
        {
            selectionInfo.selectedShapeIndex = selectionInfo.mouseOverShapeIndex;
            shapeChangedSinceLastRepaint = true;
        }
    }

    Shape SelectedShape
    {
        get
        {
            return polygonCreator.shapes[selectionInfo.selectedShapeIndex];
        }
    }


    public class SelectionInfo
    {
        public int pointIndex = -1;
        public bool mouseIsOvePoint;
        public bool pointIsSelected;
        public Vector3 positionAtStartOfDrag;


        public int lineIndex = -1;
        public bool mouseIsOnLine;

        public int selectedShapeIndex;
        public int mouseOverShapeIndex;
    }
}
