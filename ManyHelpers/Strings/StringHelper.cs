using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ManyHelpers.Strings {
    public class StringHelper {
        public static string StringToCSVCell(string str) {
            if (!string.IsNullOrEmpty(str)) {
                bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"));
                if (mustQuote) {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("\"");
                    foreach (char nextChar in str) {
                        sb.Append(nextChar);
                        if (nextChar == '"')
                            sb.Append("\"");
                    }
                    sb.Append("\"");
                    return sb.ToString();
                }
            }
   

            return str;
        }

        public static string GetOnlyPositiveNumbers(String str) {
            if (!string.IsNullOrEmpty(str)) {
                StringBuilder sb = new StringBuilder();
                foreach (char c in str) {
                    if ((c >= '0' && c <= '9')) {
                        sb.Append(c);
                    }
                }
                return sb.ToString();
            }
            return String.Empty;
        }

        public static string GetOnlyNumbers(String str, bool includePoints = false) {
            if (!string.IsNullOrEmpty(str)) {
                var matches = Regex.Matches(str, @"-?\d+");

                if (includePoints) {
                    matches = Regex.Matches(str, @"[+-]?([0-9]*[.])?[0-9]+");
                }

                StringBuilder sb = new StringBuilder();
                foreach (var c in matches) {
                    sb.Append(c.ToString());
                }
                return sb.ToString();
            }

            return String.Empty;
        }

        public static int[] GetNumbers(String str) {
            if (!string.IsNullOrEmpty(str)) {
                var matches = Regex.Matches(str, @"-?\d+");

                int[] numbers = matches.Select(x => int.Parse(x.ToString())).ToArray();
                return numbers;
            }

            return null;
        }

        public static string RemoveSpecialCharacters(String str, string except = "") {
            if (!string.IsNullOrEmpty(str)) {
                StringBuilder sb = new StringBuilder();
                foreach (char c in str) {
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == ' ' || except.Contains(c)) {
                        sb.Append(c);
                    }
                }
                return sb.ToString();
            }
     
            return string.Empty;
        }

        public static string RemoveAccents(string str) {
            if(string.IsNullOrEmpty(str)) return "";

            return new string(str
                .Normalize(NormalizationForm.FormD)
                .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                .ToArray());
        }

        public static string ShowOnlyLastsChar(string str, int showCount = 4) {
            var newStr = str;

            if (!string.IsNullOrEmpty(str) && str.Length > showCount) {
                var xs = "";

                for (var i = 0; i < str.Length - showCount; i++) {
                    xs += "*";
                }

                newStr = $"{xs}{str.Substring(str.Length - showCount)}";
            }

            return newStr;
        }

        public static string DescriptCPF(string cpf) {
            var xs = "";
            if (!string.IsNullOrEmpty(cpf)) {
                foreach (var num in cpf) {
                    switch (num) {
                        case 'a':
                            xs += "0";
                            break;
                        case 'I':
                            xs += "1";
                            break;
                        case 'z':
                            xs += "2";
                            break;
                        case 'W':
                            xs += "3";
                            break;
                        case 'q':
                            xs += "4";
                            break;
                        case 'S':
                            xs += "5";
                            break;
                        case 'p':
                            xs += "6";
                            break;
                        case 'T':
                            xs += "7";
                            break;
                        case 'b':
                            xs += "8";
                            break;
                        case 'R':
                            xs += "9";
                            break;
                        default:
                            if (char.IsNumber(num)) {
                                xs += num;
                            }

                            break;
                    }
                }

            }

            return xs;
        }

        public static string Anonimizar(string doc) {
            var newDoc = doc;
            if (!string.IsNullOrEmpty(doc) && doc.Count() > 4) {
                var xs = "";

                for (var i = 0; i < doc.Count() - 4; i++) {
                    xs += "*";
                }

                newDoc = $"{xs}{doc.Substring(doc.Count() - 4)}";
            }

            return newDoc;
        }

        public static bool IsCnpj(string cnpj) {
            cnpj = GetOnlyPositiveNumbers(cnpj);
            if (string.IsNullOrEmpty(cnpj)) {
                return false;
            }

            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;
            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito += resto.ToString();

            return cnpj.EndsWith(digito);
        }

        public static bool IsCpf(string cpf) {
            cpf = GetOnlyPositiveNumbers(cpf);

            if (string.IsNullOrEmpty(cpf)) {
                return false;
            }
            if (IsRepetedNumbers(cpf)) {
                return false;
            }
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;
            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;
            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito += resto.ToString();
            return cpf.EndsWith(digito);
        }

        public static string FormatCPF(string cpf) {
            var msk = "";
            if (IsCpf(cpf)) {
                return Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
            }

            return msk;
        }

        public static string FormatCNPJ(string cnpj) {
            var msk = "";
            if (IsCnpj(cnpj)) {
                return Convert.ToUInt64(cnpj).ToString(@"00\.000\.000\/0000\-00");
            }

            return msk;
        }

        public static (string Tipo, string Telefone) ValidaTelefone(string ddd, string telefone) {
            var telValido = ("", "");

            if (!string.IsNullOrEmpty(telefone) && telefone.StartsWith(ddd)) {
                telefone = telefone.Substring(ddd.Count());
            }

            if (!string.IsNullOrEmpty(telefone) && telefone.Length >= 8 && telefone.Length <= 11 && !IsRepetedNumbers(telefone) && !IsSequenceTo8Or9(telefone)) {
                var phone = "";
                var tipo = "";

                if (telefone.Length == 8) {
                    if (telefone.First() > '5') {
                        phone = $"{ ddd}9{telefone}";
                        tipo = "Celular";
                    } else {
                        phone = $"{ddd}{telefone}";
                        tipo = "Fixo";
                    }
                }
                if (telefone.Length == 9) {
                    phone = $"{ddd}{telefone}";
                    tipo = "Celular";
                }
                if (telefone.Length == 10) {
                    if (telefone.ToCharArray()[2] > '5') {
                        phone = telefone.Insert(2, "9");
                        tipo = "Celular";
                    } else {
                        phone = telefone;
                        tipo = "Fixo";
                    }
                }
                if (telefone.Length == 11) {
                    phone = telefone;
                    tipo = "Celular";
                }


                telValido = (tipo, phone);
            }

            return telValido;
        }

        public static string DecodeToUTF8(string str) {
            if (string.IsNullOrEmpty(str)) return null;
            byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
            str = System.Text.Encoding.UTF8.GetString(bytes);

            byte[] utf8_Bytes = new byte[str.Length];
            for (int i = 0; i < str.Length; ++i) {
                utf8_Bytes[i] = (byte)str[i];
            }

            return System.Text.Encoding.UTF8.GetString(utf8_Bytes, 0, utf8_Bytes.Length);
        }


        public static string CreateMD5(string input) {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create()) {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++) {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static bool IsRepetedNumbers(string phone) {
            var isRepeted = true;
            for (int i = 0; i < phone.Count() - 1; i++) {
                if (phone[i] != phone[i + 1]) {
                    isRepeted = false;
                    break;
                }
            }


            return isRepeted;
        }

        public static bool IsSequenceTo8Or9(string phone) {
            return phone == "12345678" || phone == "123456789";
        }

        public static bool IsConsecutive(String str) {
            // variable to store starting number 
            int start = 0;

            // length of the input String 
            int length = str.Length;

            // find the number till half of the String 
            for (int i = 0; i < length / 2; i++) {

                // new String containing the starting 
                // substring of input String 
                String new_str = str.Substring(0, i + 1);

                // converting starting substring into number 
                int num = int.Parse(new_str);

                // backing up the starting number in start 
                start = num;

                // while loop until the new_String is  
                // smaller than input String 
                while (new_str.Length < length) {

                    // next number 
                    num++;

                    // concatenate the next number 
                    new_str = new_str + String.Join("", num);
                }

                // check if new String becomes equal to 
                // input String 
                if (new_str.Equals(str))
                    return true;
            }

            // if String doesn't contains consecutive numbers 
            return false;
        }

        public static string ParseHTML(string html) {
            var messageParsed = html;

            if (!string.IsNullOrEmpty(html)) {
                if (messageParsed.Contains("<html>") || messageParsed.Contains("<body>")) {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    if (html.Contains("<body>")) {
                        var htmlBody = doc.DocumentNode.SelectSingleNode("//body");
                        messageParsed = htmlBody.InnerText.Trim();
                    } else {
                        var htmlBody = doc.DocumentNode;
                        messageParsed = htmlBody.InnerText.Trim();
                    }


                    byte[] bytes = Encoding.Default.GetBytes(messageParsed);
                    messageParsed = Encoding.UTF8.GetString(bytes);
                    messageParsed = System.Net.WebUtility.HtmlDecode(messageParsed);
                }
            }

            return messageParsed;
        }

        public static string CreateNumCodeBar(string num) {
            string NumBar = "";

            if (!string.IsNullOrEmpty(num) && num.Count() == 44) {
                var campo1 = $"{num.Substring(0, 11)}";
                var dig1 = DigitoM10(campo1);
                var campo2 = num.Substring(11, 11);
                var dig2 = DigitoM10(campo2);
                var campo3 = num.Substring(22, 11);
                var dig3 = DigitoM10(campo3);
                var campo4 = num.Substring(33);
                var dig4 = DigitoM10(campo4);

                NumBar = $"{campo1}{dig1}{campo2}{dig2}{campo3}{dig3}{campo4}{dig4}";
            }


            return NumBar;
        }

        public static long DigitoM10(string str) {
            long i = 2;
            long sum = 0;
            long res = 0;
            long fim = 0;
            foreach (char c in str.ToCharArray()) {
                res = Convert.ToInt64(c.ToString()) * i;
                sum += res > 9 ? (res - 9) : res;
                i = i == 2 ? 1 : 2;
            }
            fim = 10 - (sum % 10);
            if (fim >= 10) { fim = 0; };
            return fim;
        }

        public static string Base64Encode(string str) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(str);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData) {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        } 
    }
}
