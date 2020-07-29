/*
Radial Layout Group by Just a Pixel (Danny Goodayle) - http://www.justapixel.co.uk
Copyright (c) 2015

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

      actual website the code is found is http://www.justapixel.co.uk/2015/09/14/radial-layouts-nice-and-simple-in-unity3ds-ui-system/
      GitHub Repo: https://gist.github.com/DGoodayle/aa62a344aa83e5342175


      Goodayle, D [Just a Pixel] (2015) Radial Layout Group [Source code]. http://www.justapixel.co.uk
*/

// For our Arc-type, we used Danny Goodayle's radial layout group, with the
// necessary changes made to accomodate to our design needs.
// Created by: Danny Goodayle
// Modified by: Zahara M. Spilka
// Date Created: 2015
// Date Modified Updated: 07/29/2020

namespace UnityEngine.UI
{
  //This adds the layout to the layout Group
    [AddComponentMenu("Layout/Circular Layout Group", 155)]


    public class CircularLayout : LayoutGroup
    {
      // Sets up user/procter display aspects of the layout.
      // Fdistance is the distance between bins
      // The user/procter can set the layout's minimum angle, maximum angle, and
      // starting angle as any value between 0 and 360.
      // The user/procter can also set the layout to be the only viable one,
      // however this feature is obsolete due to the nature of our interface's
      // design. I left it in the file, in the event that this not oboslete when
       // it comes to the CalculateCircular function assginging the letter bin's
       // positions around the circlular layout.

        public float fDistance;
        [Range(0f, 360f)]
        public float MinAngle, MaxAngle, StartAngle;
        public bool OnlyLayoutVisible = false;

        //The following functions, depending on their name, call the
        // CaclualeCirlcle function

        protected override void OnEnable()
        {
            base.OnEnable();
            CalculateCircular();
        }
        public override void SetLayoutHorizontal()
        {
        }
        public override void SetLayoutVertical()
        {
        }
        public override void CalculateLayoutInputHorizontal()
        {
            CalculateCircular();
        }
        public override void CalculateLayoutInputVertical()
        {
            CalculateCircular();

        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            CalculateCircular();
        }
#endif

// When this function is called it checks if the layout has any children, then
// assigns the amount of children to the int variable, ChildrenToFormat.
// Next, the function sets the offset angles value so that the children, when
// assginend psotions are evenly spaced out in the layout.
// Then, the function sets the value of the current angle to the start angle;
// the fucntion does this so that when it begins assinging the bins' positions,
// the first bin is placed at the start of the layout. For instance, if the
// start angle was 90 degrees, the first child of the layout is placed at the 90
// degree angle.
// Next the function looks for the virtual stylus and assigns its position.
// Lastly, the function runs through the key-bins and assigns them positions
// along the layout in realation to the order they assigned. For example, the
// ABCD bin is assgined the first position, then the EFGH is assigned the next
// position, and so on and so forth until all the layouts children are placed
// in/along the Arc layout.
        void CalculateCircular()
        {
            m_Tracker.Clear();
            if (transform.childCount == 0)
                return;
            int ChildrenToFormat = 0;
            // if (OnlyLayoutVisible)
            // {
            //     for (int i = 0; i < transform.childCount; i++)
            //     {
            //         RectTransform child = (RectTransform)transform.GetChild(i);
            //         if ((child != null) && child.gameObject.activeSelf)
            //             ++ChildrenToFormat;
            //     }
            // }
            // else
            // {
                ChildrenToFormat = transform.childCount;
            // }

            float fOffsetAngle = (MaxAngle - MinAngle) / (transform.childCount - 1);

            float fAngle = StartAngle;


            for (int i = 0; i < transform.childCount; i++)
            {
              // Because the first child of the layout is a virtual stylus, the
              // function finds and assigns the game object to this child. Then,
              // the funciton sets the child's postion to be the center of the
              // circular layout.
                if (i == 0)
                {
                    Debug.LogWarning("Child is Stylus");
                    GameObject child = GameObject.FindGameObjectWithTag("CircularStylus");
                    child.transform.position = transform.position;
                }
                else
                {
                    RectTransform child = (RectTransform)transform.GetChild(i);
                    // if ((child != null) && (!OnlyLayoutVisible || child.gameObject.activeSelf))
                    if ((child != null) && (child.gameObject.activeSelf))
                    {
                        //Adding the elements to the tracker stops the user from
                        // modifiying their positions via the editor.


                        // This also sets the bins up so that they are at an
                        // angle around the layout. For example, the middle bin
                        // is turned 90 degress so that it appears horizontal
                        // rather than vertical.

                        m_Tracker.Add(this, child,
                        DrivenTransformProperties.Anchors |
                        DrivenTransformProperties.AnchoredPosition |
                        DrivenTransformProperties.Pivot);
                        Vector3 vPos = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad), 0);
                        child.localPosition = vPos * fDistance;
                        Quaternion vRot = Quaternion.Euler(0, 0, fAngle - 90);
                        child.localRotation = vRot;

                        //Force objects to be center aligned, this can be
                        // changed however I'd suggest you keep all of the
                        // objects with the same anchor points.
                        child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);
                        fAngle += fOffsetAngle;
                    }
                }
            }
        }
    }
}
