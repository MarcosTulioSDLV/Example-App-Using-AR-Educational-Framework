using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeralObjectsContainer{
    //NOTA: classe container com objetos gerias e objetos empty associados, para os objetos que vao ser escalados pelo framework (na classe ButtonOperations e ChangeElement), lembrando que cada objeto será vinculado como filho dum objeto vazio empty colocado no ponto do marcador e que será escalado ao inves do proprio objeto, permitindo escalar o objeto de jeito proporcional sem se sobrepor no marcador. 

    private GameObject originalObject;//armazena o objeto original, se usa como identificador
    public GameObject OriginalObject{ get {return originalObject;}}
    private GameObject emtyObject;//armazena o objeto empty associado ao objeto orignal, o qual vai ser escalado ao inves do objeto original, e portanto permitirá fazer a escala de jeito proporcional sem se sobrepor no marcador
    public GameObject EmtyObject { get {return emtyObject;}}

    public GeralObjectsContainer()
    {
        originalObject = new GameObject();
        emtyObject = new GameObject();
    }

    public GeralObjectsContainer(GameObject originalObject, GameObject emtyObject)
    {
        this.originalObject = originalObject;
        this.emtyObject = emtyObject;
    }
}
