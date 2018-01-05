using UnityEditor;

namespace Foxsys.ApartmentEditor
{
    public class Toolbar
    {
        private readonly ApartmentEditorWindow _ParentWindow;
        private readonly Skin _CurrentSkin;
        public void Draw()
        {
            if (_ParentWindow.CurrentState == ApartmentEditorWindow.EditorWindowState.Normal)
            {
                EditorGUILayout.BeginVertical();
                RecenterButton();
                CreateRoomButton();
                VertButton();
                DoorButton();
                WindowButton();
                EditorGUILayout.EndVertical();
            }
        }

        public void CreateRoomButton()
        {
            if (EditorUIUtils.ButtonWithFallback(
                    _CurrentSkin.IconCreateRoom,
                    "Create Room",
                    _CurrentSkin.MiniButtonStyle
                ))
            {
                _ParentWindow.CreateRoomStateBegin();
            }
        }

        public void RecenterButton()
        {
            if (EditorUIUtils.ButtonWithFallback(
                    _CurrentSkin.IconRecenter,
                    "Recenter",
                    _CurrentSkin.MiniButtonStyle
                ))
            {
                _ParentWindow.Grid.Recenter();
            }
        }

        public void DoorButton()
        {
            if (EditorUIUtils.ButtonWithFallback(
                null,
                "AD",
                _CurrentSkin.MiniButtonStyle
            ))
            {
                _ParentWindow.AddObjectStateBegin(ObjectsManager.Mode.Doors);
            }
        }
        public void WindowButton()
        {
            if (EditorUIUtils.ButtonWithFallback(
                null,
                "AW",
                _CurrentSkin.MiniButtonStyle
            ))
            {
                _ParentWindow.AddObjectStateBegin(ObjectsManager.Mode.Windows);
            }
        }
        public void VertButton()
        {
            if (EditorUIUtils.ButtonWithFallback(
                null,
                "AV",
                _CurrentSkin.MiniButtonStyle
            ))
            {
                _ParentWindow.AddObjectStateBegin(ObjectsManager.Mode.Vert);
            }
        }
        public Toolbar(ApartmentEditorWindow parentWindow)
        {
            _ParentWindow = parentWindow;
            _CurrentSkin = SkinManager.Instance.CurrentSkin;
        }
    }
}