using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMaps : MonoBehaviour
{
    public void Open(){
	Application.OpenURL("http://maps.google.com/maps?q=Trident+Academy+of+Technology");
    }
}
