using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JiraBoardConfiguration
{
    public string name;
    public JiraBoardColumnConfig columnConfig;
    
    [System.Serializable]
    public class JiraBoardColumnConfig
    {
        public List<JiraBoardColumn> columns;
    }
}
