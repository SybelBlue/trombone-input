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

TODO: How to properly cite in code that we used code found on the web as our
      starting point for this layout
      actual website the code is found is http://www.justapixel.co.uk/2015/09/14/radial-layouts-nice-and-simple-in-unity3ds-ui-system/
      GitHub Repo: https://gist.github.com/DGoodayle/aa62a344aa83e5342175
*/

namespace UnityEngine.UI{
  [AddComponentMenu("Layout/Circular Layout Group", 155)]


public class CircularLayout : LayoutGroup {


    public float fDistance;
    [Range(0f,360f)]
    public float MinAngle, MaxAngle, StartAngle;
    public bool OnlyLayoutVisible = false;

   protected override void OnEnable()
   {
     base.OnEnable();
     CalculateCircular();
   }
    public override void SetLayoutHorizontal()
    {
      // SetChildrenAlongAxis(0);
    }
    public override void SetLayoutVertical()
    {
      // SetChildrenAlongAxis(0);
    }
    public override void CalculateLayoutInputHorizontal()
    {
      // CalculateCircular();
      // base.CalculateLayoutInputHorizontal();
      // CalcAlongAxis(0);
      CalculateCircular();
    }
    public override void CalculateLayoutInputVertical()
    {
        // CalculateCircular();
        // CalcAlongAxis(0);
        CalculateCircular();

    }
    #if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        CalculateCircular();
    }
    #endif

    void CalculateCircular()
    {
        m_Tracker.Clear();
        if (transform.childCount == 0)
            return;
        int ChildrenToFormat =0;
        if(OnlyLayoutVisible)
        {
          for (int i = 0; i< transform.childCount; i++)
          {
            RectTransform child = (RectTransform)transform.GetChild(i);
            if((child !=null) && child.gameObject.activeSelf)
              ++ChildrenToFormat;
          }
        }
        else
        {
          ChildrenToFormat = transform.childCount;
        }




        float fOffsetAngle = (MaxAngle - MinAngle) / (transform.childCount -1);
        // float fOffsetAngle = (MaxAngle - MinAngle) / ChildrenToFormat;

        float fAngle = StartAngle;


        for (int i = 0; i < transform.childCount; i++)
        {
          // Debug.LogWarning(transform.position);
          // Debug.LogWarning(i);
          if (i==0)
          {
            Debug.LogWarning("Child is Stylus");
            GameObject child = GameObject.FindGameObjectWithTag("CircularStylus");
            // Vector3 vPos = new Vector3(0, 0, 0);
            child.transform.position = transform.position;
          }
          else
          {
           //TODO: make so that it recgonzies when the child is the stylus and skips it
            RectTransform child = (RectTransform)transform.GetChild(i);
            if ((child != null) && (!OnlyLayoutVisible || child.gameObject.activeSelf))
            {
                //Adding the elements to the tracker stops the user from modifiying their positions via the editor.
                m_Tracker.Add(this, child,
                DrivenTransformProperties.Anchors |
                DrivenTransformProperties.AnchoredPosition |
                DrivenTransformProperties.Pivot);
                Vector3 vPos = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad), 0);
                child.localPosition = vPos * fDistance;
                 Quaternion vRot = Quaternion.Euler(0, 0, fAngle-90);
                // Vector3 vRot = new Vector3(0f,0f,fAngle);
                child.localRotation = vRot;
                // Vector3 vScal = new Vector3(0.99f, 1.0f, 0);
                // child.localScale = vScal;



                //Force objects to be center aligned, this can be changed however I'd suggest you keep all of the objects with the same anchor points.
                child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);
                fAngle += fOffsetAngle;
            }
          }
        }

    }
  }
}
