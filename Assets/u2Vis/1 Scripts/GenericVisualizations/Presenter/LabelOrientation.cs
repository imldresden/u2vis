namespace u2vis
{
    /// <summary>
    /// Dtermines the orientation of a label in realtion to axis.
    /// </summary>
    public enum LabelOrientation : int
    {
        /// <summary>
        /// Values are parallel to the axis, e.g., for a horizontal axis labels are also horizontal.
        /// </summary>
        Parallel = 0,
        /// <summary>
        /// Values are rotated 45° counterclockwiese to the axis.
        /// </summary>
        Diagonal = 1,
        /// <summary>
        /// Values are orthogonal to the axis, e.g., for a horizontal axis labels are vertical.
        /// </summary>
        Orthogonal = 2,
    }
}
