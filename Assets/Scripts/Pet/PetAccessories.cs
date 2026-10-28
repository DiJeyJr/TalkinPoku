using UnityEngine;

/// Va en la mascota. Dibuja la ropa que tiene puesta, una por ranura, en el ancla que dice la
/// apariencia de la etapa: asi el sombrero sigue en la cabeza cuando Poku evoluciona.
[RequireComponent(typeof(SpriteRenderer))]
public class PetAccessories : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Inventory inventario;
    [SerializeField] private PetMoodController cara;
    // Dormido esta hecho una bolita: la ropa quedaria flotando, asi que se esconde.
    [SerializeField] private NeedsSystem necesidades;

    private SpriteRenderer cuerpo;
    private SpriteRenderer[] puestos;

    private void Awake()
    {
        cuerpo = GetComponent<SpriteRenderer>();
        int ranuras = System.Enum.GetValues(typeof(AccessorySlot)).Length;
        puestos = new SpriteRenderer[ranuras];
        for (int i = 0; i < ranuras; i++)
        {
            GameObject hijo = new GameObject("Accesorio_" + (AccessorySlot)i);
            hijo.transform.SetParent(transform, false);
            puestos[i] = hijo.AddComponent<SpriteRenderer>();
            puestos[i].sortingLayerID = cuerpo.sortingLayerID;
        }
    }

    private void OnEnable()
    {
        inventario.Cambiado += Refrescar;
        cara.AparienciaCambiada += Refrescar;
        necesidades.DormidoCambiado += AlDormir;
    }

    private void OnDisable()
    {
        inventario.Cambiado -= Refrescar;
        cara.AparienciaCambiada -= Refrescar;
        necesidades.DormidoCambiado -= AlDormir;
    }

    private void Start()
    {
        Refrescar();
    }

    private void AlDormir(bool dormido) => Refrescar();

    private void Refrescar()
    {
        PetAppearance apariencia = cara.Apariencia;
        Sprite referencia = apariencia != null ? apariencia.Normal : null;

        for (int i = 0; i < puestos.Length; i++)
        {
            ItemDefinition item = inventario.PuestoEn((AccessorySlot)i);
            bool mostrar = item != null && item.SpriteMundo != null && referencia != null && !necesidades.Dormido;
            puestos[i].enabled = mostrar;
            if (!mostrar) continue;

            puestos[i].sprite = item.SpriteMundo;
            // Capa negativa (las alas) va detras del cuerpo; el resto, adelante.
            puestos[i].sortingOrder = cuerpo.sortingOrder + (item.Capa < 0 ? item.Capa : 1 + item.Capa);
            ItemDefinition.AjusteEtapa ajuste = item.AjusteDe(apariencia);
            float escalaAjuste = ajuste.escala > 0f ? ajuste.escala : 1f;
            Vector2 lugar = (Vector2)PuntoDelSprite(referencia, apariencia.Ancla(item.Ranura))
                + item.Desplazamiento * apariencia.EscalaAccesorios + ajuste.desplazamiento;
            puestos[i].transform.localPosition = lugar;
            puestos[i].transform.localScale = Vector3.one * (apariencia.EscalaAccesorios * item.EscalaMundo * escalaAjuste);
        }
    }

    // Pasa una fraccion del dibujo a coordenadas locales, teniendo en cuenta el pivot del sprite.
    private static Vector3 PuntoDelSprite(Sprite sprite, Vector2 fraccion)
    {
        Vector2 pixeles = Vector2.Scale(fraccion, sprite.rect.size) - sprite.pivot;
        return pixeles / sprite.pixelsPerUnit;
    }
}
