using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStackedVis
{
    List<Vector3> GetSegmentStartList(Vector3 normDirectionVector);
}
