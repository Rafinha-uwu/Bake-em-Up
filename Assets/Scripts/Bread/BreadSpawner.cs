using UnityEngine;

public class BreadSpawner : MonoBehaviour
{
    [SerializeField] private GameObject Bread;
    [SerializeField] private GameObject Baggette;
    [SerializeField] private GameObject Cupcake;
    [SerializeField] private GameObject Pastery;
    [SerializeField] private GameObject Queso;
    [SerializeField] private GameObject Donut;
    public void SpawnBread()
    {

        Instantiate(Bread, new Vector3(-0.601000011f, 1.85585773f, 2.24099994f), Bread.gameObject.transform.rotation);
        Instantiate(Baggette, new Vector3(-0.397000015f, 1.85012937f, 2.29164863f), Baggette.gameObject.transform.rotation);
        Instantiate(Pastery, new Vector3(-0.137065172f, 1.92411208f, 2.34260011f), Pastery.gameObject.transform.rotation);
        Instantiate(Cupcake, new Vector3(0.0829999968f, 1.94000006f, 2.25900006f), Cupcake.gameObject.transform.rotation);
        Instantiate(Queso, new Vector3(0.31099999f, 1.94700003f, 2.25900006f), Queso.gameObject.transform.rotation);
        Instantiate(Donut, new Vector3(0.536000013f, 1.90900004f, 2.26600003f), Donut.gameObject.transform.rotation);

    }

}
