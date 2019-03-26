using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChScript : MonoBehaviour {

	private List<GameObject> models;
    //default index of the model
    private int selectionIndex = 0;
    
    private void Start(){
        models = new List<GameObject>();
        foreach(Transform t in transform){
            models.Add(t.gameObject);
            t.gameObject.SetActive(false);
        }
        
        models[selectionIndex].SetActive(true);
    }
    
    private void Update(){
        transform.Rotate(new Vector3(0.0f, Input.GetAxis("Mouse X"),0.0f));
    }
    
    public void Select(int index){
        if(index == selectionIndex)
            return;
        
        if(index < 0 || index >= models.Count)
            return;
        
        models[selectionIndex].SetActive(false);
        selectionIndex = index;
        models[selectionIndex].SetActive(true);
    }
}
