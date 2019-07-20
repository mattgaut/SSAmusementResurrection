using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFormula : MonoBehaviour {

    [SerializeField] CharacterFormula formula;
    [SerializeField] Character test;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(formula.GetValue(test));
        
    }
}
