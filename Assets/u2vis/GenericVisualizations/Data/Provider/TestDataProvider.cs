using System;

namespace u2vis
{
    /// <summary>
    /// DataProvider that generates some randomized values that can be used for test purposes.
    /// </summary>
    public class TestDataProvider : AbstractDataProvider
    {
        /// <summary>
        /// The DataSet of this data provider.
        /// </summary>
        private DataSet _data = null;
        /// <summary>
        /// Gets the DataSet which this DataProvider provides.
        /// </summary>
        public override DataSet Data => _data;
        /// <summary>
        /// Creates a new instance of the TestDataProvider class.
        /// </summary>
        public TestDataProvider()
        {
            _data = CreateTestData();
        }
        /// <summary>
        /// Creates the radomized test DataSet of this provider.
        /// </summary>
        /// <returns>The resulting DataSet.</returns>
        public DataSet CreateTestData()
        {
            Random r = new Random();
            DataSet data = new DataSet();
            data.Add(new StringDimension("Categories (attr 1)", null));
            data.Add(new IntegerDimension("Some Integers (attr 2)", null));
            data.Add(new FloatDimension("Some Floats (attr 3)", null));
            data.Add(new BooleanDimension("Some Bools (attr 4)", null));

            for (int i = 0; i < 100; i++)
            {
                data[0].Add(i.ToString());
                data[1].Add(r.Next(10));
                data[2].Add((float)r.NextDouble());
                data[3].Add((i % 2) > 0);
            }

            return data;
        }
        /// <summary>
        /// Initializes this TestDataProvider by creating the test DataSet.
        /// </summary>
        public void Initialize()
        {
            _data = CreateTestData();
        }
    }
}
