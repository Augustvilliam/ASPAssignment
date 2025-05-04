10 veckor senare, 1dm längre skägg. kronsik migrän. 

NOTERINGAR:
Vi pratade snabbt om min ajaxlösning för project och member vyn, den var en mardröm att jobba med. kom med ungefär 1000000bugs och 15 miljoner rader javascript. 
jag valde dock att implementera mer klassisk MVC för te.x settings(förutom navigeringen innuti själva settings där körde jag SPA igen), admins roll managersida osv. kände att det var alldeless för mycket. Jag kommer nog hålla mig till rå MVC i framtiden. C# -> JS
Jag försökte implementera en glömt lösenords lösning, men det blev pankaka av allt och jag sket i det. det sätter ett felmeddelande att det ej är implementerat 
Jag var också fruktansvärt lat med "Add members flikten till mellan menun av Project -> meny -> modal och skickar bara användaren direkt till editProjectmodalen oavsätt om man väljer Add members, eller edit project. 
tror aldrig jag haft så mycket problem i mitt liv som jag hade med just Memberpickern, först hade jag någon hembyggd version tillsammans med chatGPT som funkade lika bra som en bil utan kammrem, sen släpptes videon med sökning RÄDDAD tänkte jag, men tog mig år och dagar att få den att fungera och är det som i särklass tog längst tid att få att fungera. 

Jag slopade tredjeparts inloggningen från adminvyn(Se Stor disclaimer under bugs) man kan fortfarande skapa och ge roller med admin behörighet eller bara rent av admin-rollen till tredjepartsanvändare 
Jag flyttade också hela tredjeparts delen till en egen vy, blev på tok för plottrigt att ha det direkt i login formuläret. och tog bort vyn för registrering via tredje part eftersom att ett konto ändå görs om du väljer att logga in med te.x google. 
___________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________

KVARVARANDE BUGGS:

Formuläret i settings tappar hälften av sin data ibland när man navigerar mellan vyerna där. Fick aldrig löst det.
Status fälteten i projectview behåller inte "Active" status för styling endast. jag satt säkert ett dygn med google och chat gpt men inget löste sig.
Tredjepart login med facebook funkade typ två gånger, och sen bah "Nej". borde ha tagit bort det från vyn men när jag upptäkte att det inte funkade längre (inlämningsdag) Så orkade jag verkligen inte bry mig längre. Man skickas ändå bara tillbaka till Loginvyn. Git, Google och microsoft funkar fint. 

STOR DISCLAIMER: Jag har försökt säkert 300 lösningar för att blocka admins från att använda vanliga portalen men fick det aldrig att fungera, en admin kan alltså logga in genom vanliga portalen. 
Vanliga användare kan dock inte logga in via adminportal. Jag har säkert missuppfattat uppgiften här men båda går ju ändå till samma vy, men gör ändå inget eftersom att de som är låst till admins är bortsållat med rollhantering och Claims ändå. 
___________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________

AI GREJOR: 
Användt både ChatGPT o4 och den nya o4-high-mini, och Github Co-pilot till balanade resultat. precis i vanlig ordning vill chatgpt integrerta 300 olika nuget paket och hjälpbibliotek om man är det minsta otydlig med problem. 
och ju mer kod som kom desto mer tappade all ai funktion sin nytta.
I vanlig ordning har den lyckats fucka upp de allra simplaste sakerna.
dock har high-mini varit ett steg mot rätt riktning, men tappar snabbt bollen.  

Använd dessa flitigt i denna uppgiften för typ all buggfix. Men oftast har det blivit mer soppa en lösning, och man har fått se en rad av kod som en lösning när den tyckte att en hel a4 var lämpligt att genera. 
Modalhandler, Memberpicker, AdminController, Cookie delarna är där det används mest. 

skulle uppskatta att 90% presentationslagret(Dock inte HTML och css) av koden någon gång har körts genom chat gpt, för antingen förklaring, optimering eller bugfix, dock korrigeras dessa bitarna för hand(copy pasta direkt är som att dansa på ett infält)

Dock de som är rakt av copypastat från Chatgpt bottarna bör vara noterade. 
