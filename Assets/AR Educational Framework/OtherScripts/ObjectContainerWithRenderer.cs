using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ObjectContainerWithRenderer {
    //classe para guardar objeto, seu componente renderer (necessario para buscá), e materiais. NOTA: será necessario para processos relacionados com fazer Reset nos materiais de objetos (cor).

    [SerializeField] private GameObject objectWithRenderer;//armazena o objeto com componente renderer
    public GameObject ObjectWithRenderer { get {return objectWithRenderer;}}
    [SerializeField] private Renderer componentRenderer;//armazena o componente renderer do objeto (usado para buscá)
    public Renderer ComponentRenderer{ get {return componentRenderer;}}
    [SerializeField] private Material[] materialsVector;//armazena o vetor de materiais do componente renderer
    public Material[] MaterialsVector { get{return materialsVector;}}

    public ObjectContainerWithRenderer(GameObject myObject, Renderer componentRenderer, Material[] myMaterials)
    {
        this.objectWithRenderer = myObject;
        this.componentRenderer = componentRenderer;
        this.materialsVector = myMaterials;
    }

}
