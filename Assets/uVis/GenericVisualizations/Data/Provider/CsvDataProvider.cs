using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UVis
{
    public class CsvDataProvider : AbstractDataProvider
    {
        #region Private Fields
        [SerializeField]
        private TextAsset _csvFile = null;
        [SerializeField]
        private char _seperator = ';';
        [SerializeField, Tooltip("Indicates if the data dimensions in the csv are orientatet in rows or columns.")]
        private bool _rowBasedLayout = false;
        [SerializeField, Tooltip("Indicates if the second field of either row or column (depending on the orientation) contains the data type. If not the data type is automatically derived from the the values of the corresponding dimension.")]
        private bool _seconFieldContainesDataType = false;

        private DataSet _data = null;
        #endregion

        #region Public Properties
        public override DataSet Data
        {
            get
            {
                // If lazy initialized, be sure to not call OnDataChanged
                if (_data == null)
                    LoadData_internal();
                return _data;
            }
        }
        public int NumberOfColumns { get; private set; }
        public int NumberOfRows { get; private set; }

        public char Seperator
        {
            get { return _seperator; }
            set
            {
                if (_seperator == value)
                    return;
                _seperator = value;
                _data = null;
            }
        }

        public bool RowBasedLayout
        {
            get { return _rowBasedLayout; }
            set
            {
                if (_rowBasedLayout == value)
                    return;
                _rowBasedLayout = value;
                _data = null;
            }
        }

        public bool SecondRowContainsDataType
        {
            get { return _seconFieldContainesDataType; }
            set
            {
                if (_seconFieldContainesDataType == value)
                    return;
                _seconFieldContainesDataType = value;
                _data = null;
            }
        }
        #endregion

        #region Private Methods
        private void LoadData_internal()
        {
            if (_csvFile == null)
                Debug.LogError("CSV text asset can't ne null!");
            var text = _csvFile.text;
            if (text == null || text == "")
                Debug.LogError("CSV file doesn't exist or is empty");
            string[] lines = text.Trim().Split('\n');

            _data = new DataSet();
            if (_rowBasedLayout)
                LoadDimensionRows(lines);
            else
                LoadDimensionColumns(lines);
        }

        private void LoadDimensionRows(string[] lines)
        {
            int offset = _seconFieldContainesDataType ? 2 : 1;
            string[] line0 = lines[0].Trim().Split(_seperator);
            NumberOfColumns = line0.Length - offset;
            NumberOfRows = lines.Length;
            
            for (int l = 0; l < lines.Length; l++)
            {
                var fields = lines[l].Trim().Split(_seperator);
                string caption = fields[0].Trim();
                DataType dataType = DataType.Undefinded;
                if (_seconFieldContainesDataType)
                    dataType = (DataType)Enum.Parse(typeof(DataType), fields[1].Trim());
                else
                    dataType = GetTypeFromString(fields[1].Trim());
                int length = fields.Length - offset;
                switch (dataType)
                {
                    case DataType.Boolean:
                        var bValues = new bool[length];
                        for (int i = 0; i < length; i++)
                            bValues[i] = Boolean.Parse(fields[i + offset].Trim());
                        _data.Add(new BooleanDimension(caption, bValues));
                        break;
                    case DataType.Integer:
                        var iValues = new int[length];
                        for (int i = 0; i < length; i++)
                            iValues[i] = Int32.Parse(fields[i + offset].Trim());
                        _data.Add(new IntegerDimension(caption, iValues));
                        break;
                    case DataType.Float:
                        var fValues = new float[length];
                        for (int i = 0; i < length; i++)
                            fValues[i] = Single.Parse(fields[i + offset].Trim());
                        _data.Add(new FloatDimension(caption, fValues));
                        break;
                    case DataType.String:
                        var sValues = new string[length];
                        for (int i = 0; i < length; i++)
                            sValues[i] = fields[i + offset].Trim();
                        _data.Add(new StringDimension(caption, sValues));
                        break;
                }
            }
        }

        private void LoadDimensionColumns(string[] lines)
        {
            int offset = _seconFieldContainesDataType ? 2 : 1;
            // contains the captions
            string[] line0 = lines[0].Trim().Split(_seperator);
            // contains either the first row of values or the data types
            string[] line1 = lines[1].Trim().Split(_seperator);

            // for each dimension, get the caption and the data type
            for (int i = 0; i < line0.Length; i++)
            {
                string caption = line0[i].Trim();
                DataType dataType = DataType.Undefinded;
                if (_seconFieldContainesDataType)
                    dataType = (DataType)Enum.Parse(typeof(DataType), line1[i].Trim());
                else
                    dataType = GetTypeFromString(line1[i].Trim());

                switch (dataType)
                {
                    case DataType.Boolean:
                        _data.Add(new BooleanDimension(caption, null));
                        break;
                    case DataType.Integer:
                        _data.Add(new IntegerDimension(caption, null));
                        break;
                    case DataType.Float:
                        _data.Add(new FloatDimension(caption, null));
                        break;
                    case DataType.String:
                        _data.Add(new StringDimension(caption, null));
                        break;
                }
            }

            // now read the values
            for (int l = offset; l < lines.Length; l++)
            {
                string[] line = lines[l].Trim().Split(_seperator);
                for (int i = 0; i < line.Length; i++)
                    switch (_data[i].DataType)
                    {
                        case DataType.Boolean:
                            _data[i].Add(bool.Parse(line[i].Trim()));
                            break;
                        case DataType.Integer:
                            if (line[i].Trim().Equals(String.Empty))
                                _data[i].Add(Int32.MinValue);
                            else
                                _data[i].Add(Int32.Parse(line[i].Trim()));
                            break;
                        case DataType.Float:
                            try
                            {
                                if (line[i].Trim().Equals(String.Empty))
                                    _data[i].Add(float.NaN);
                                else
                                    _data[i].Add(float.Parse(line[i].Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture));
                            } catch (Exception e)
                            {
                                Debug.Log("line " + l + ", column " + i + ": " + line[i].Trim());
                                throw e;
                            }
                            break;
                        case DataType.String:
                            _data[i].Add(line[i].Trim());
                            break;
                    }
            }
        }

        private DataType GetTypeFromString(string s)
        {
            if (Boolean.TryParse(s, out bool b))
                return DataType.Boolean;
            if (Int32.TryParse(s, out int i))
                return DataType.Integer;
            if (Single.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float f))
                return DataType.Float;
            return DataType.String;
        }
        #endregion

        #region Public Methods

        public void Initialize(TextAsset csvFile, char seperator = ';', bool rowBasedLayout = false, bool secondFieldContainsDataType = false)
        {
            _csvFile = csvFile;
            _seperator = seperator;
            _rowBasedLayout = rowBasedLayout;
            _seconFieldContainesDataType = secondFieldContainsDataType;
            _data = null;
        }
        /// <summary>
        /// Loads (or reloads) the data from the specified file.
        /// </summary>
        public void LoadData()
        {
            LoadData_internal();
            OnDataChanged();
        }
        #endregion
    }
}
