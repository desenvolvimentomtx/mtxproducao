using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MatrizTributaria.Controllers
{
    public class Criptografia
    {
       
       

        //vai receber o texto para criptografar
        public static string Encrypt(string text)
        {
            //bloco try-catch
            try {
                //verifica a string - se nao vazia inicia a criptografia
                if (!string.IsNullOrEmpty(text)) 
                {

                    // Cria instancias de vetores de bytes com as chaves                
                    byte[] bKey = Convert.FromBase64String("2020pR3c1s0MTX01"); //2019pRecIsoTaX01;          //CHAVE 1 - Necessária para descriptografar
                    byte[] bIV = Convert.FromBase64String("hAC8hMf3N5Zb/DZhkdIEldpp"); //CHAVE 2 - Necessária para descriptografar
                    byte[] bText = new UTF8Encoding().GetBytes(text); //trasnforma em bytes o texto passado no parametro

                    // Instancia a classe de criptografia Rijndael
                    Rijndael rijndael = new RijndaelManaged();

                    // Define o tamanho da chave "256 = 8 * 32"                
                    // Lembre-se: chaves possíves:                
                    // 128 (16 caracteres), 192 (24 caracteres) e 256 (32 caracteres)                
                    rijndael.KeySize = 128;

                    // Cria o espaço de memória para guardar o valor criptografado:                
                    MemoryStream mStream = new MemoryStream();

                    // Instancia o encriptador                 
                    CryptoStream encryptor = new CryptoStream(
                        mStream,
                        rijndael.CreateEncryptor(bKey, bIV),
                        CryptoStreamMode.Write);

                    // Faz a escrita dos dados criptografados no espaço de memória
                    encryptor.Write(bText, 0, bText.Length);

                    // Despeja toda a memória.                
                    encryptor.FlushFinalBlock();

                    // Pega o vetor de bytes da memória e gera a string criptografada                
                    return Convert.ToBase64String(mStream.ToArray());
                }
                else
                {
                   
                    return null;
                }

            } catch (Exception ex)
            {
                // Se algum erro ocorrer, dispara a exceção    
                System.Diagnostics.Debug.Write(ex.ToString() + "\n");
                throw new ApplicationException("Erro ao criptografar", ex);

            }

           
        }


        public static string Decrypt(string text) 
        {
            try 
            {
                // Se a string não está vazia, executa a criptografia           
                if (!string.IsNullOrEmpty(text))
                {
                    // Cria instancias de vetores de bytes com as chaves                
                    byte[] bKey = Convert.FromBase64String("2020pR3c1s0MTX01");
                    byte[] bIV = Convert.FromBase64String("hAC8hMf3N5Zb/DZhkdIEldpp");
                    byte[] bText = Convert.FromBase64String(text);

                    // Instancia a classe de criptografia Rijndael                
                    Rijndael rijndael = new RijndaelManaged();

                    // Define o tamanho da chave "256 = 8 * 32"                
                    // Lembre-se: chaves possíves:                
                    // 128 (16 caracteres), 192 (24 caracteres) e 256 (32 caracteres)                
                    rijndael.KeySize = 128;

                    // Cria o espaço de memória para guardar o valor DEScriptografado:               
                    MemoryStream mStream = new MemoryStream();

                    // Instancia o Decriptador                 
                    CryptoStream decryptor = new CryptoStream(
                        mStream,
                        rijndael.CreateDecryptor(bKey, bIV),
                        CryptoStreamMode.Write);

                    // Faz a escrita dos dados criptografados no espaço de memória   
                    decryptor.Write(bText, 0, bText.Length);
                    // Despeja toda a memória.                
                    decryptor.FlushFinalBlock();
                    // Instancia a classe de codificação para que a string venha de forma correta         
                    UTF8Encoding utf8 = new UTF8Encoding();
                    // Com o vetor de bytes da memória, gera a string descritografada em UTF8       
                    return utf8.GetString(mStream.ToArray());
                }
                else {
                    // Se a string for vazia retorna nulo                
                    return null;
                }

            }
            catch (Exception ex)
            {
                // Se algum erro ocorrer, dispara a exceção            
                throw new ApplicationException("Erro ao descriptografar", ex);
            }
        
        }

        private void write(string texto)
        {
            System.Diagnostics.Debug.Write(texto + "\n");
        }


    }
}