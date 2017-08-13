using UnityEngine;
namespace Nox7atra.ApartmentEditor
{
    public class NormalStateHotkeys : StateHotkeys
    {
        #region attributes
        NormalState _Parent;
        #endregion

        #region public methods
        public override void Use(Event @event)
        {
            switch (@event.keyCode)
            {
        
                case KeyCode.Delete:
                    Delete();
                    break;
                case KeyCode.S:
                    _Parent.SaveCurrentApartment();
                    break;
            }
        }
        #endregion

        #region hotkeys
        void Delete()
        {
            _Parent.DeleteSelected();
        }
        #endregion

        #region constructor
        public NormalStateHotkeys(NormalState parent)
        {
            _Parent = parent;
        }
        #endregion
    }
}