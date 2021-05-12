namespace SilK.Unturned.Extras.UI
{
    public class UISessionOptions
    {
        public bool EndOnDeath { get; set; } = false;

        /// <summary>
        /// All effects are cleared when the arena is preparing to start - including UI effects.
        /// This option allows for ending UI sessions when this clearing occurs.
        /// </summary>
        public bool EndOnArenaClear { get; set; } = false;
    }
}
