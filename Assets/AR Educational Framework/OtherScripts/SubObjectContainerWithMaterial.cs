using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SubObjectContainerWithMaterial {
    //classe container para manter sub-objetos com seu vetor de materiais associado. NOTA: necessario para a classe ChangeElement usando a funcionalidade de mudar materiais (cor). 

    [SerializeField] private GameObject subObjectWithMaterial;//objeto que possui diretamente componente Renderer (MeshRenderer, SkinnedMeshRenderer etc) com materiais
    public GameObject SubObjectWithMaterial { get {return subObjectWithMaterial;} set { subObjectWithMaterial = value;} }
    [SerializeField] private Material[] materialsVector;//vetor de materiais associado
    public Material[] MaterialsVector{get{return materialsVector;} set {materialsVector = value;}}

    public SubObjectContainerWithMaterial()
    {
        MaterialsVector = new Material[0];
    }

}
