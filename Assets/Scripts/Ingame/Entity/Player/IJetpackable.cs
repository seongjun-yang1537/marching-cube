namespace Ingame
{
    public interface IJetpackable
    {
        public bool CanJetpack();
        public void ActivateJetpack();
        public void DeactivateJetpack();
    }
}