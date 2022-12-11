function ziskatNavigaci(napoveda, navigace = false, adresa = null) {
    let html = "";
    let podsekce = Object.keys(napoveda);
    if (podsekce.length > 0 && podsekce[0] == "&Nazev") {
        podsekce = podsekce.reverse();
        podsekce.pop();
        podsekce = podsekce.reverse();
    }

    if (podsekce.length != 0) {
        if (navigace)
            html += '<ul id="napoveda_menu_seznam" class="skryty">';
        else
            html += "<ul>";
        podsekce.forEach(sekce => {
            if (adresa == null) {
                html += '<li><a href="/Napoveda/' + sekce + '">' + napoveda[sekce]["&Nazev"];
                html += ziskatNavigaci(napoveda[sekce], false, sekce);
            } else {
                let aktualniAdresa = adresa + "-" + [sekce];
                html += '<li><a href="/Napoveda/' + aktualniAdresa + '">' + napoveda[sekce]["&Nazev"];
                html += ziskatNavigaci(napoveda[sekce],false, aktualniAdresa);
            }
            html += "</a></li>";
        });
        html += "</ul>";
    }

    return html;
}
    
let ROUTE = document.getElementById("ROUTE").innerHTML.split("-");

function load(sekce) {
    $.getJSON("../napoveda/napoveda.json",
        function(napoveda) {
            const prehravacObsah = document.getElementById("napoveda_prehravac_obsah");
            const prehravacNadpis = document.getElementById("napoveda_prehravac_nadpis");
            let html = "../napoveda/html/";
            let cesta = "";
            let podKategorie;

            let aktualniNapoveda = napoveda;

            document.getElementById("napoveda_navigace_menu").innerHTML =
                document.getElementById("napoveda_navigace_menu").innerHTML + ziskatNavigaci(napoveda, true);

            try {
                sekce.forEach(rt => {
                    aktualniNapoveda = aktualniNapoveda[rt];
                    html += rt + "/";
                    cesta += aktualniNapoveda["&Nazev"];
                    if (rt != sekce[sekce.length - 1])
                        cesta += " > ";
                });

                prehravacNadpis.innerHTML = aktualniNapoveda["&Nazev"];
                fetch(html + "Index.html")
                    .then(response => response.text())
                    .then(text => prehravacObsah.innerHTML = text);
                document.getElementById("napoveda_navigace_menu_cesta").innerHTML = cesta;

                let podnavigace = ziskatNavigaci(aktualniNapoveda, false, document.getElementById("ROUTE").innerHTML);
                if (podnavigace.trim().length > 0)
                    podnavigace = "<h2>Na toto navazuje...</h2>" + podnavigace;

                document.getElementById("napoveda_podsekce").innerHTML = podnavigace;

                $("#napoveda_navigace_rozbalit").click(() => {
                    if (document.getElementById("napoveda_menu_seznam").className === "skryty") {
                        document.getElementById("napoveda_menu_seznam").className = "";
                    } else {
                        document.getElementById("napoveda_menu_seznam").className = "skryty";
                    }
                });
            }

            catch (chyba) {
                prehravacNadpis.innerHTML = "Došlo k chybě!";
                prehravacObsah.innerHTML = "<i>Obsah této nápovědy není nyní přístupný!</i>";
            }
        }
    );
}

$("#napoveda_navigace_zpet").click(() => {
    if (ROUTE.length != 1) {
        document.getElementById("ROUTE").innerHTML = document.getElementById("ROUTE").innerHTML.replace(ROUTE[ROUTE.length - 1], "");
        document.getElementById("ROUTE").innerHTML =
            document.getElementById("ROUTE")
                .innerHTML.substring(0, document.getElementById("ROUTE").innerHTML.length - 1);

        ROUTE = document.getElementById("ROUTE").innerHTML.split("-");
        load(ROUTE);
    }
});

$('body').on('click',
    'img',
    (e) => {
        window.location.href = e.target.src;
    });

load(ROUTE);
