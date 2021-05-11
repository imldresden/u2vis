namespace u2vis.NodeLink
{
    public static class IdGenerator
    {
        private static int _idCounter = 0;

        public static int GenId()
        {
            return _idCounter++;
        }
    }
}
