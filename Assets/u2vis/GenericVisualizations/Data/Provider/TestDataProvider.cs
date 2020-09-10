using System;

namespace u2vis
{
    public class TestDataProvider : AbstractDataProvider
    {
        private DataSet _data = null;

        public override DataSet Data => _data;

        public TestDataProvider()
        {
            _data = CreateTestData();
        }

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

        public void Initialize()
        {
            _data = CreateTestData();
        }
    }
}
