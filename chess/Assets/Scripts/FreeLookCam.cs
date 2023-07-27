using UnityEngine;

public class FreeLookCam : MonoBehaviour
{
    public Cinemachine.CinemachineFreeLook freeLookCam;

    private bool isRightClicking = false;

    private void Update()
    {
        // Check if the right mouse button is pressed
        if (Input.GetMouseButtonDown(1))
        {
            isRightClicking = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isRightClicking = false;
        }

        // Enable/disable camera orbit with right-click
        if (freeLookCam)
        {
            if (isRightClicking)
            {
                freeLookCam.m_XAxis.m_InputAxisName = "Mouse X";
                freeLookCam.m_YAxis.m_InputAxisName = "Mouse Y";
            }
            else
            {
                freeLookCam.m_XAxis.m_InputAxisValue = 0f;
                freeLookCam.m_YAxis.m_InputAxisValue = 0f;
            }
        }
    }
}
