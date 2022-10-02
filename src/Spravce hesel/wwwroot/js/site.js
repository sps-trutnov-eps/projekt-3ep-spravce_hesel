function NahodneCislo(max) {
    return Math.floor(Math.random() * max);
}

function zobrazeni_hesla() {
    let heslo = document.getElementById("gen_vystup");
    let tlacitko = document.getElementById("zobrazeni_hesla_tlacitko");

    if (tlacitko.className == "icon-eye") {    
        heslo.type = "text";
        tlacitko.className = "icon-eye-off";
    }
    else {
        heslo.type = "password";
        tlacitko.className = "icon-eye";
    };
}

function delka_hesla_aktualizace() {
    document.getElementById("posuvnikvystup").value = document.getElementById("gen_delka").value;
}

function delka_hesla_zapis() {
    document.getElementById("gen_delka").value = document.getElementById("posuvnikvystup").value;
}

function generace(form) {
    delka_hesla_aktualizace()
    const skupiny_symbolu = [
        ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"],
        ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"],
        ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"],
        [",", ".", "_", "-", "!", "^", "°", ";", "?", "$", "€", "<", ">", "*", "/"]
    ]
    let pouzivane_symboly = [];
    let vysledne_heslo = "";
    let vyber = [
        form.gen_mala.checked,
        form.gen_velka.checked,
        form.gen_cisla.checked,
        form.gen_znaky.checked
    ]
    let count = 0;

    vyber.forEach(skupina => {
        if (skupina)
            pouzivane_symboly.push(skupiny_symbolu[count]);
        count += 1;
    });

    let skupina_symbolu;
    count = 0;
    while (count != form.gen_delka.value) {
        skupina_symbolu = pouzivane_symboly[NahodneCislo(pouzivane_symboly.length)];
        vysledne_heslo = vysledne_heslo.concat(skupina_symbolu[NahodneCislo(skupina_symbolu.length)]);
        count += 1;
    };

    document.getElementById("gen_vystup").value = vysledne_heslo;
}
