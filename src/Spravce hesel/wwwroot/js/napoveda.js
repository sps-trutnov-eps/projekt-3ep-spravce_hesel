$.getJSON("../json/napoveda.json", function (napoveda) {

    const ROUTE = document.getElementById("ROUTE").innerHTML;
    const prehravac_obsah = document.getElementById("napoveda_prehravac_obsah");
    const prehravac_nadpis = document.getElementById("napoveda_prehravac_nadpis")

    const sekce = ROUTE.split("-");
    let aktualniNapoveda = napoveda;

    sekce.forEach(rt => {
        aktualniNapoveda = aktualniNapoveda[rt];
    })

    prehravac_nadpis.innerHTML = aktualniNapoveda["Nazev"];
    prehravac_obsah.innerHTML = aktualniNapoveda["Obsah"];

})

