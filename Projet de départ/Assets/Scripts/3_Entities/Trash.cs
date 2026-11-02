using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Déchet.
//
// Les personnages "propres" ramassent les déchets qu'ils détectent, tandis que les personnages malpropres lancent de
// nouveau déchets dans l'environnement. La propriété "IsAvailable" indique si le déchet peut être ramassé ou non.
//
// Vous n'avez pas à toucher cette classe pour le travail.
public class Trash : MonoBehaviour, IDestination
{
    [Header("Visual")]
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Transform meshTransform;
    [SerializeField] private TrashVariant[] variants;

    #region (Ne pas toucher) Editor related stuff.
    
#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private int variantIndex = -1;
#endif
    
    #endregion

    public Vector3 Position => transform.position;
    public bool IsAvailable => isActiveAndEnabled;

    private void OnEnable()
    {
        // Use random visual when activated.
        SetVariant(variants.Random());
    }

    private void SetVariant(TrashVariant variant)
    {
        // Cancel if references are invalid.
        if (!meshTransform || !meshFilter || !meshRenderer)
            return;

        #region (Ne pas toucher) Editor related stuff.
        
#if UNITY_EDITOR
        // Record change if inside editor.
        if (!EditorApplication.isPlaying)
        {
            Undo.RecordObject(gameObject, "Changed trash visual");
        }
#endif
        
        #endregion
        
        // Update mesh, material and scale.
        if (meshFilter.sharedMesh != variant.mesh)
            meshFilter.sharedMesh = variant.mesh;
        if (meshRenderer.sharedMaterial != variant.material)
            meshRenderer.sharedMaterial = variant.material;
        meshTransform.localScale = Vector3.one * variant.scale;
    }

    #region (Ne pas toucher) Editor related stuff.
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        // When updated inside editor, change visual.
        SetVariant(variants[variantIndex >= 0 ? variantIndex : 0]);
    }
#endif
    
    #endregion

    [Serializable]
    public sealed class TrashVariant
    {
        public Mesh mesh;
        public Material material;
        public float scale = 1f;
    }
}