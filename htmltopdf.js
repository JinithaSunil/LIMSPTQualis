const puppeteer = require('puppeteer');

(async () => {
    const browser = await puppeteer.launch();
    const page = await browser.newPage();

    await page.goto('http://192.168.29.179:82/PO/PO-80.html', {
        waitUntil: 'networkidle0'
    });

    await page.pdf({
        path: 'output.pdf',
        format: 'A4',
        printBackground: true
    });

    await browser.close();
})();