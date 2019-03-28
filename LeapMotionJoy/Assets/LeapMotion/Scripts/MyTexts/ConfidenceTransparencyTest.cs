/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

/** 
 * Updates the hand's opacity based it's confidence rating. 
 * Attach to a HandModel object assigned to the HandController in a scene.
 */
public class ConfidenceTransparencyTest : MonoBehaviour
{

    void Update()
    {
        Hand leap_hand = GetComponent<HandModel>().GetLeapHand();
        float confidence = leap_hand.Confidence;
        Vector norm = leap_hand.PalmNormal; Vector3 normUnity = new Vector3(norm.x, norm.y, norm.z); normUnity = Vector3.ProjectOnPlane(normUnity, Vector3.forward);
        float angle = Vector3.Angle(Vector3.down, normUnity);
        Vector hight = leap_hand.PalmPosition; float vertical = Mathf.Round(hight.y); float horizontal = Mathf.Round(hight.x);
        if (leap_hand != null)
        {
            //if (((angle <= 10) || (angle >= 80 && angle <= 100) || (angle >= 170)) && leap_hand.TimeVisible >= 0.5f)
            if (leap_hand.TimeVisible >= 0.2f)
            {
                //LeapManager.log.Add(confidence + "\t" + leap_hand.SphereRadius + "мм\t" + angle + "°\t" + vertical + "мм\t" + horizontal + "мм\t");
                LeapManager.log.Add(confidence + "\t" + angle);
            }
            Renderer[] renders = GetComponentsInChildren<Renderer>();
            foreach (Renderer render in renders)
                SetRendererAlpha(render, confidence);
        }
    }

    protected void SetRendererAlpha(Renderer render, float alpha)
    {
        Color new_color = render.material.color;
        new_color.a = alpha;
        render.material.color = new_color;
    }
}
