// number to string, pluginized from http://stackoverflow.com/questions/5529934/javascript-numbers-to-words

window.num2str = function (num) {
    return window.num2str.convert(num);
}

window.num2str.ones = ['', 'One', 'Two', 'Three', 'Four', 'Five', 'Six', 'Seven', 'Eight', 'Nine'];
window.num2str.tens = ['', 'One', 'Twenty', 'Thirty', 'Forty', 'Fifty', 'Sixty', 'Seventy', 'Eighty', 'Ninty'];
window.num2str.teens = ['Ten', 'Eleven', 'Twelve', 'Thirteen', 'Fourteen', 'Fifteen', 'Sixteen', 'Seventeen', 'Eighteen', 'Nineteen'];

window.num2str.convert_crores = function(num) {
    if (num >= 10000000) {
        return this.convert_crores(Math.floor(num / 10000000)) + " Crore " + this.convert_millions(num % 10000000);
    }
    else {
        return this.convert_millions(num);
    }
}

window.num2str.convert_millions = function(num) {
    if (num >= 100000) {
        return this.convert_millions(Math.floor(num / 100000)) + " Lakh " + this.convert_thousands(num % 100000);
    }
    else {
        return this.convert_thousands(num);
    }
}

window.num2str.convert_thousands = function(num) {
    if (num >= 1000) {
        return this.convert_hundreds(Math.floor(num / 1000)) + " Thousand " + this.convert_hundreds(num % 1000);
    }
    else {
        return this.convert_hundreds(num);
    }
}

window.num2str.convert_hundreds = function(num) {
    if (num > 99) {
        return this.ones[Math.floor(num / 100)] + " Hundred " + this.convert_tens(num % 100);
    }
    else {
        return this.convert_tens(num);
    }
}

window.num2str.convert_tens = function(num) {
    if (num < 10) return this.ones[num];
    else if (num >= 10 && num < 20) return this.teens[num - 10];
    else {
        return this.tens[Math.floor(num / 10)] + " " + this.ones[num % 10];
    }
}

window.num2str.convert = function(num) {
    if (num == 0) return "Zero";
    else return this.convert_crores(num);
}
