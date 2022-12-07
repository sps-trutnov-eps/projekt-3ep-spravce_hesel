function ziskatPodkategorie(napoveda, adresa = null) {
    let html = "";
    let podsekce = Object.keys(napoveda);
    podsekce = podsekce.reverse();
    podsekce.pop();
    podsekce = podsekce.reverse();

    if (podsekce.length != 0) {
        if (adresa == null) {
            html += "<ul>";
            podsekce.forEach(sekce => {
                html += "<li>" + napoveda[sekce]["Nazev"];
                html += ziskatPodkategorie(napoveda[sekce]);
                html += "</li>";
            });
            html += "</ul>";
        } else {
            html += "<ul>";
            podsekce.forEach(sekce => {
                let aktualniAdresa = adresa + "-" + [sekce];
                html += '<li><a href="/Napoveda/' + aktualniAdresa + '">' + napoveda[sekce]["Nazev"];
                html += ziskatPodkategorie(napoveda[sekce], aktualniAdresa);
                html += "</a></li>";
            });
            html += "</ul>";
        }
    }

    return html;
}

function ziskatNavigaci(napoveda) {
    let html = "";
    let podsekce = Object.keys(napoveda);

    if (podsekce.length != 0) {
        html += '<ul id="napoveda_menu_seznam" class="skryty">';
        podsekce.forEach(sekce => {
            html += '<li><a href="/Napoveda/' + sekce + '">' + napoveda[sekce]["Nazev"];
            html += ziskatPodkategorie(napoveda[sekce], sekce);
            html += "</li>";
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
                document.getElementById("napoveda_navigace_menu").innerHTML + ziskatNavigaci(napoveda);


            try {
                sekce.forEach(rt => {
                    aktualniNapoveda = aktualniNapoveda[rt];
                    html += rt + "/";
                    cesta += aktualniNapoveda["Nazev"];
                    if (rt != sekce[sekce.length - 1])
                        cesta += " > ";
                });

                prehravacNadpis.innerHTML = aktualniNapoveda["Nazev"];
                fetch(html + "Index.html")
                    .then(response => response.text())
                    .then(text => prehravacObsah.innerHTML = text);
                document.getElementById("napoveda_navigace_menu_cesta").innerHTML = cesta;

                document.getElementById("napoveda_podsekce").innerHTML = ziskatPodkategorie(aktualniNapoveda);

                $("#napoveda_navigace_rozbalit").click(() => {
                    if (document.getElementById("napoveda_menu_seznam").className === "skryty") {
                        document.getElementById("napoveda_menu_seznam").className = "";
                    } else {
                        document.getElementById("napoveda_menu_seznam").className = "skryty";
                    }
                });
            }

            catch (chyba) {
                console.log(chyba);
                prehravacNadpis.innerHTML = "Došlo k chybě!";
                prehravacObsah.innerHTML = "<i>Obsah této nápovědy není nyní přístupný!</i>";
            }
        }
    );
}

$("#napoveda_navigace_zpet").click(() => {
    if (ROUTE.length != 1) {
        ROUTE.pop();
        load(ROUTE);
    }
});


load(ROUTE);
