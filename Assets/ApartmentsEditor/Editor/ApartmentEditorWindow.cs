using UnityEditor;
using UnityEngine;
namespace Assets.ApartmentsEditor.Editor
{
    public class ApartmentEditorWindow : EditorWindow
    {
        #region factory
        [MenuItem("Window/ApartmentEditor")]
        public static void Create()
        {
            var window = GetWindow<ApartmentEditorWindow>("ApartmentEditor");
            window.Show();
        }
        #endregion

        #region attributes

        private Grid m_Grid;

        #endregion

        #region engine methods

        private void OnGUI()
        {
            m_Grid.Draw(position.size);
        }

        #endregion

        #region constructors

        public ApartmentEditorWindow()
        {
            m_Grid = new Grid();
        }

        #endregion
    }
}
