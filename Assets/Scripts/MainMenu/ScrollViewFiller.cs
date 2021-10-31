using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewFiller : MonoBehaviour {
    public GameObject CommunicatorCanvas;

    void Update() {
        // DestroyAllChildren();
        FillScrollViewWithClients();
    }

    void DestroyAllChildren() {
        while (transform.childCount > 0) {
            Destroy(transform.GetChild(0).gameObject);
        }
    }

    void FillScrollViewWithClients() {
        foreach (var client in CommunicatorCanvas.GetComponent<HostCommunicator>().GetClientList()) {
            GameObject newGameObject = new GameObject();
            Text myText = newGameObject.AddComponent<Text>();
            myText.text = "nice";
            newGameObject.transform.SetParent(this.transform);
        }
    }
}
