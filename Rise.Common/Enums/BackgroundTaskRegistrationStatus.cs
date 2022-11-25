namespace Rise.Common.Enums
{
    public enum BackgroundTaskRegistrationStatus
    {
        /// <summary>
        /// Indicates that the background task is registered
        /// successfully.
        /// </summary>
        Successful,
        /// <summary>
        /// The background task cannot be registered because
        /// of set policies.
        /// </summary>
        NotAllowed,
        /// <summary>
        /// The background task has not registered successfully.
        /// </summary>
        Failed,
        /// <summary>
        /// The background task is already registered.
        /// </summary>
        AlreadyExists
    }
}
