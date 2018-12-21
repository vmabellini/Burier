# Burier
A simple stenography tool to hide secret text data inside image files

You can use Burier to hide text data inside an image file, as long the image is large enough to have space to store it :)
There's a ultra-secret option for you who wants to hide some information without I/O on disk. You can direct input the text you want to hide, and use a command to read it direct from the console interface.

You can also use a password to encrypt the data and add a new layer of protection, being able to copy the output image freely.

Type --help to see all the options!

Example:

Hide the text stored on the text.txt file inside the image original.png, generating a new image called modified.png

dotnet Burier.dll --write --imagepath=original.png --datapath=text.txt --outputpath=modified.png

Read the text store on the modified.png file and writing it to a unburied.txt file

dotnet Burier.dll --read --imagepath=modified.png --outputpath=unburied.txt

Hide the text stored on the text.txt file inside the image original.png, generating a new image called modified.png with a secret password 12345678

dotnet Burier.dll --write --imagepath=original.png --datapath=text.txt --outputpath=modified.png --secretkey=12345678

Read the text store on the modified.png file (encrypted with the secret password 12345678) and writing it to a unburied.txt file 

dotnet Burier.dll --read --imagepath=modified.png --outputpath=unburied.txt --secretkey=12345678

If you are really paranoid about safety, you can even write/read secret data without needing to read from a file/restore to a file using the --ultra-secret mode. In this mode you write the text on a line and read it direct from the console.
