// This file gives has the virtual stylus follow and mimic the movements of the
// user's stylus. This is implemented on the Arc-Type layout, making it easier
// for the user to see which bin they are pointing to, and how the angle they
// are holding the stylus relates to the bin they are pointing to.
// Written by Zahara M. Spilka
// Date Created:
// Date Last Updated: 07/29/2020

﻿using UnityEngine;

namespace Utils
{
#pragma warning disable 649
    public class FollowTheStylus : MonoBehaviour
    {
      // instantiates the transform of the physical stylus.
        [SerializeField]
        private Transform stylusTransform; //(LC)

        // When the file is ran, it first finds and assigns the transform of the
        // physical stylus to the instantiated transform variable.

        void Start()
        {
            stylusTransform = GameObject.FindWithTag("StylusTag").transform;
        }

        // Update is called once per frame
        // When this file updates, it first finds the virtual stylus' rotation
        // transform data. Then, it updates the virtual stylus' rotation using
        // the physical stylus’s x rotation and the virtual stylus' y and z
        // rotation. This way, the virtual stylus matches--or follows--the
        // physical stylus' change in rotation along the X axis while
        // maintaining its original y and z rotations.


        void Update()
        {
            Vector3 transAngles = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(stylusTransform.rotation.eulerAngles.x, transAngles.y, transAngles.z);
        }
    }
}
