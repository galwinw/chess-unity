using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Buoyancyy : MonoBehaviour {

    public Transform[] floaters;
    public float underWaterDrag = 3f;
    public float underWaterAngularDrag = 1f;

    public float airDrag = 0f;
    public float airAngularDrag = 0.5f;

    public float floatingPower = 1f;

    public float waterHeight = 0f;
    Rigidbody m_Rigidbody;

    int floatersUnderwater;
    bool underwater;

    void Start() {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        floatersUnderwater = 0;
        for (int i = 0; i < floaters.Length; i++) {
            float diff = floaters[i].position.y - waterHeight;

            if (diff < 0) {
                m_Rigidbody.AddForceAtPosition(Vector3.up * floatingPower *Mathf.Abs(diff), floaters[i].position, ForceMode.Force);
                floatersUnderwater++;
                if (!underwater) {
                    underwater = true;
                    SwitchState(underwater);
                }
            } 
        }
        if (underwater && floatersUnderwater == 0) {
            underwater = false;
            SwitchState(underwater);
        }
         
    }

    void SwitchState(bool isUnderwater) {
        if (isUnderwater) {
            m_Rigidbody.drag = underWaterDrag;
            m_Rigidbody.angularDrag = underWaterAngularDrag;
        } else {
            m_Rigidbody.drag = airDrag;
            m_Rigidbody.angularDrag = airAngularDrag;
        }
    }
}