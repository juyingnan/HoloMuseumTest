/*==============================================================================
Copyright (c) 2017 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;
using Vuforia;

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
///
/// Changes made to this file could be overwritten when upgrading the Vuforia version.
/// When implementing custom event handler behavior, consider inheriting from this class instead.
/// </summary>
public class CheckpointTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    public StoryLine storyline;
    public int currentStep = 0;
    public AudioSource audioSource;
    public GameObject MainFrame;
    private bool isAudioPlayed = false;
    private int mainFrameCounter = 0;
    private Vector3 mainFrameOffset = new Vector3(0f, 0f, 0f);

    #region PROTECTED_MEMBER_VARIABLES

    protected TrackableBehaviour mTrackableBehaviour;
    protected TrackableBehaviour.Status m_PreviousStatus;
    protected TrackableBehaviour.Status m_NewStatus;

    #endregion // PROTECTED_MEMBER_VARIABLES

    #region UNITY_MONOBEHAVIOUR_METHODS

    protected virtual void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

        // set default offset
        if (MainFrame != null)
        {
            mainFrameOffset = MainFrame.transform.position;
    } }

    protected virtual void OnDestroy()
    {
        if (mTrackableBehaviour)
            mTrackableBehaviour.UnregisterTrackableEventHandler(this);
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        m_PreviousStatus = previousStatus;
        m_NewStatus = newStatus;

        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NO_POSE)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
            OnTrackingLost();
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }

    #endregion // PUBLIC_METHODS

    #region PROTECTED_METHODS

    protected virtual void OnTrackingFound()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = true;

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Enable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;

        if (storyline != null && currentStep >= 0)
        {
            storyline.UpdateHintText(currentStep);
        }


        if (audioSource != null && audioSource.isPlaying == false && isAudioPlayed == false)
        {
            audioSource.Play();
            isAudioPlayed = true;
        }

        //# main frame tracking
        if (MainFrame != null && mainFrameCounter < 33)
        {
            var temp_Position = MainFrame.transform.position;
            MainFrame.transform.position = this.transform.position;
            MainFrame.transform.rotation = this.transform.rotation;
            MainFrame.transform.Rotate(90, 0, 0);
            //MainFrame.transform.forward = this.transform.forward;
            MainFrame.transform.Translate(new Vector3(0.3f, -1.75f, -0.1f));
            var offset = temp_Position - MainFrame.transform.position;
            if (offset.magnitude > 0.25f)
            {
                mainFrameCounter = 0;
            }
            else
            {
                mainFrameCounter += 1;
            }
            if (mainFrameCounter >= 3)
            {
                //var mainFrameRendererComponents = MainFrame.GetComponentsInChildren<Renderer>(true);
                //var mainFrameColliderComponents = MainFrame.GetComponentsInChildren<Collider>(true);
                //var mainFrameCanvasComponents = MainFrame.GetComponentsInChildren<Canvas>(true);

                //// Enable rendering:
                //foreach (var component in mainFrameRendererComponents)
                //    component.enabled = true;

                //// Enable colliders:
                //foreach (var component in mainFrameColliderComponents)
                //    component.enabled = true;

                //// Enable canvas':
                //foreach (var component in mainFrameCanvasComponents)
                //    component.enabled = true;
            }
        }
    }


    protected virtual void OnTrackingLost()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = false;
    }

    #endregion // PROTECTED_METHODS
}
