# **Správce hesel**

Webová aplikace pro uschovávání a snadnou správu hesel k různým službám.

## **Funkce**

----------

- Umožňuje snadnou generaci hesel podle parametrů, které si zvolí uživatel.
- Snadná správa, úprava, mazání hesel.
- Hesla jsou uložená a bezpečně zašifrována na serveru Správce Hesel.
- Upozornění na duplicitu a krátká hesla.
- Sdílení hesel od sdílených tarifů s ostatními příslušníky.
- Optimalizováno pro použití i na mobilních zařízeních.

## **Instalace**

----------

Aplikace je postavena na frameworku **.NET 6**. *Ke stažení na stránkách [dotnet.microsoft.com](https://dotnet.microsoft.com/en-us/download/dotnet/6.0).*

Po stažení a nainstalování .NET můžete aplikaci spustit příkazem `dotnet run` v adresáři, kde se nachází soubor `Spravce hesel.csproj`.

`dotnet --project Spravce hesel.csproj`

Alternativně je možné aplikaci spustit přes *IIS Express* nebo přes vývojové prostředí *Visual Studio 2022*.

### **SASS**

----------

Aplikace používá pro styl stránek SASS. Pro jeho použití je nutné SASS přeložit na CSS.\
Pro překlad je potřeba mít nainstalovaný **Node.js** a globálně nainstalovaný modul **SASS**.

Pro více informací navštivte stránky [sass-lang.com](https://sass-lang.com/).

#### **Instalace SASS**

`npm install -g sass`

Pokud není možné nainstalovat SASS globálně, je možné jej stáhnout z [GitHub repozitáře](https://github.com/sass/dart-sass/releases) a spustit příkazem `sass` v adresáři, kde se nachází soubor `sass.bat`.

#### **Použití SASS**

Po instalaci je možné spustit SASS přikazem `sass [cesta k .scss nebo .sass souboru] [cesta kam uložit .css]`.

Například: `sass D:\mojestranka\style.scss D:\mojestranka\style.css`.\
*Po provedení příkazu se ze souboru `D:\mojestranka\style.scss` vytvoří soubor `style.css` v adresáři `D:\mojestranka\`.*
