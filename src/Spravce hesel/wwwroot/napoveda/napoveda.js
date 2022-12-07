let ROUTE = document.getElementById("ROUTE").innerHTML.split("-");

function load(sekce) {
    $.getJSON("../napoveda/napoveda.json", function (napoveda) {
        const prehravac_obsah = document.getElementById("napoveda_prehravac_obsah");
        const prehravac_nadpis = document.getElementById("napoveda_prehravac_nadpis");
        let html = "../napoveda/html/";
        let cesta = "";

        let aktualniNapoveda = napoveda;

        try {
            sekce.forEach(rt => {
                aktualniNapoveda = aktualniNapoveda[rt];
                html += rt + "/";
                cesta += aktualniNapoveda["Nazev"];
                if (rt != sekce[sekce.length - 1])
                    cesta += " > ";
            })

            prehravac_nadpis.innerHTML = aktualniNapoveda["Nazev"];
            fetch(html + "Index.html")
                .then(response => response.text())
                .then(text => prehravac_obsah.innerHTML = text);
            document.getElementById("napoveda_navigace_menu_cesta").innerHTML = cesta;

            let podsekce = Object.keys(aktualniNapoveda);
            podsekce = podsekce.reverse();
            podsekce.pop();
            podsekce = podsekce.reverse();
        }

        catch (chyba) {
            prehravac_nadpis.innerHTML = "Došlo k chybě!";
            prehravac_obsah.innerHTML = "<i>Obsah této nápovědy není nyní přístupný!</i>";
        }
    })
}

function zpet() {
    if (ROUTE.length != 1) {
        ROUTE.pop();
        load(ROUTE);
    }
}

function rozbalitMenu(tlacitko) {
    if (document.getElementById("napoveda_menu_seznam").className == "skryty") {
        document.getElementById("napoveda_menu_seznam").className = "";
        tlacitko.innerHTML = "▲";
    }
    else {
        document.getElementById("napoveda_menu_seznam").className = "skryty";
        tlacitko.innerHTML = "▼";
    }
}

load(ROUTE);
