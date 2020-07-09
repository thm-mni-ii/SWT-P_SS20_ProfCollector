using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class controls different effects on a light.
/// </summary>
public class LightController : MonoBehaviour
{
    /// <summary>
    /// Original target to look at
    /// </summary>
    [SerializeField] private Transform normalTarget;
    
    /// <summary>
    /// Original spot angel
    /// </summary>
    [SerializeField] private float normalSpotAngel;

    /// <summary>
    /// Spot angel when looking at a player
    /// </summary>
    [SerializeField] private float playerSpotAngel;

    /// <summary>
    /// Target to look at
    /// </summary>
    private Transform target;

    /// <summary>
    /// Stores the wanted spot angel
    /// </summary>
    private float targetSpotAngel;

    /// <summary>
    /// Stores the spot angel at which the change is started
    /// </summary>
    private float startSpotAngel;

    /// <summary>
    /// Stores time for spot angle change.
    /// </summary>
    private float timeParam;
    
    /// <summary>
    /// Makes the light look at the given player.
    /// </summary>
    /// <param name="player">Player to look at</param>
    public void LookAtPlayer(Transform player)
    {
        startSpotAngel = GetComponent<Light>().spotAngle;
        targetSpotAngel = playerSpotAngel;
        timeParam = 0f;
        target = player;
    }

    /// <summary>
    /// Resets the light to its original target.
    /// </summary>
    public void ResetLight()
    {
        startSpotAngel = GetComponent<Light>().spotAngle;
        targetSpotAngel = normalSpotAngel;
        timeParam = 0f;
        target = normalTarget;
    }

    /// <summary>
    /// Looks at the target, if it exists, and changes spot angle.
    /// </summary>
    private void Update()
    {
        if (target == null) return;
         
        // Update Rotation
        Quaternion OriginalRot = transform.rotation;
        transform.LookAt(target);
        Quaternion NewRot = transform.rotation;
        transform.rotation = OriginalRot;
        transform.rotation = Quaternion.Lerp(transform.rotation, NewRot, 5f * Time.deltaTime);
        
        // Update Spotlight
        timeParam += Time.deltaTime * 2f;
        GetComponent<Light>().spotAngle = Mathf.Lerp(startSpotAngel, targetSpotAngel, timeParam);
    }
}
