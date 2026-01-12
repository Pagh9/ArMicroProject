using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class HumanModel : MonoBehaviour
{
     [SerializeField] private Animator animator;

     private void OnEnable()
     {
         if (animator == null)
         {
             animator = GetComponentInChildren<Animator>();
         }
     }

     public void PlaceHumanModel([CanBeNull]ARTrackable trackableParent)
     {
        // Parrenting the model under the plane. Less stable
         transform.SetParent(trackableParent?.transform);
         animator.SetBool("IsAwake", true);
     }

   
}
