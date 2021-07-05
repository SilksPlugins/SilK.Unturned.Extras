namespace SilK.Unturned.Extras.UI
{
    /// <summary>
    /// Extra options to manage UI session functionality.
    /// </summary>
    public class UISessionOptions
    {
        /// <summary>
        /// This option allows for ending UI sessions on death.
        /// By default, UI sessions will persist through death.
        /// </summary>
        public bool EndOnDeath { get; set; } = false;

        /// <summary>
        /// All effects are cleared when the arena is preparing to start - including UI effects.
        /// This option allows for ending UI sessions when this clearing occurs.
        /// By default, UI sessions will not end on arena clearance.
        /// </summary>
        public bool EndOnArenaClear { get; set; } = false;
    }
}
