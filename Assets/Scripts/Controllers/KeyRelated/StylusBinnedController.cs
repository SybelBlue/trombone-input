using System.Collections.Generic;
using UnityEngine;

class StylusBinnedController : AmbiguousKeyController
{
    public override void SetSlant(bool forward)
    { }
    // public override float Resize(float sensorHeight)
    // {
    //   var height = sensorHeight * data.size;
    //   rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, height);
    //   return height;
    // }
    // public virtual void ResizeAll()
    // {
    //   var height = gameObject.GetComponent<RectTransform>().rect.height;
    //   var unitHeight = height/22.0f;
    //   foreach (var child in gameObject.GetComponentsInChildren<KeyController>())
    //   {
    //       child.ResizeHeight(unitHeight);
    //   }
    //
    // }

}
