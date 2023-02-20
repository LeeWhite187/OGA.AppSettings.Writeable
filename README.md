# OGA.AppSettings.Writeable
Provides write access to Json files loaded by ConfigurationBuilder

## Description
This library contains the classes and logic necessary for runtime-update of json config files that are consumed by ConfigurationBuilder.
This allows json files, such as appsettings.json, to be edited from a REST call, and changes take immediate effect.

## Installation
OGA.AppSettings.Writeable is available via NuGet:
* NuGet Official Releases: [![NuGet](https://img.shields.io/nuget/vpre/OGA.AppSettings.Writeable.svg?label=NuGet)](https://www.nuget.org/packages/OGA.AppSettings.Writeable)

## Dependencies
This library depends on:
* [OGA.SharedKernel](https://github.com/LeeWhite187/OGA.SharedKernel)
* [NewtonSoft.Json](https://github.com/JamesNK/Newtonsoft.Json)

## Usage
Here are usage examples...

### Create In-Memory Keystore with some keys
```
            // Create three keys...
            KeyStore_v2_Base.Create_New_AES_Key(Guid.NewGuid().ToString(), 256, out var k1);
            KeyStore_v2_Base.Create_New_ECDSA_KeyPair(Guid.NewGuid().ToString(), out var k2);
            KeyStore_v2_Base.Create_New_RSA_KeyPair(Guid.NewGuid().ToString(), 512, out var k3);

            // Add all three keys to a new in-memory keystore instance...
            var ks = new KeyStore_v2_Base();
            var res1 = ks.AddKey_toStore(k1);
            var res2 = ks.AddKey_toStore(k2);
            var res3 = ks.AddKey_toStore(k3);
```

### Get Oldest Active Symmetric Key in Keystore
```
            // Create a keystore with a couple of symmetric keys...
            KeyStore_v2_Base.Create_New_AES_Key(Guid.NewGuid().ToString(), 256, out var k1);
            KeyStore_v2_Base.Create_New_AES_Key(Guid.NewGuid().ToString(), 256, out var k2);

            var ks = new KeyStore_v2_Base();
            var res1 = ks.AddKey_toStore(k1);
            var res2 = ks.AddKey_toStore(k2);

            // Retrieve the oldest AES key in the keystore...
            // To query the store, we need to build a predicate filter... for AES keys.
            var filter = OGA.DomainBase.QueryHelpers.PredicateBuilder.True<KeyObject_v2>(); // Filter for symmetric keys.
            filter = filter.And<KeyObject_v2>(t => t.Is_SymmetricKey()); // Filter for enabled keys.
            filter = filter.And<KeyObject_v2>(t => t.Status == eKeyStatus.Enabled); // Filter for private keys.
            // Pass the query filter to the keystore...
            var res = ks.GetOldestKey_fromStore_byFilter(filter, out var k4);
            if (res != 1)
            {
                // Failed to locate an AES key in keystore.
                return;
            }
            
            // Do something with the retrieved key...
            var keystring = k4.PrivateKey;
```

### Save a Keystore to a File
```
            // Create a couple of keys...
            KeyStore_v2_Base.Create_New_AES_Key(Guid.NewGuid().ToString(), 256, out var k1);
            KeyStore_v2_Base.Create_New_AES_Key(Guid.NewGuid().ToString(), 256, out var k2);

            // Create a file-based keystore instance...
            // Pass in the filepath and storage password at construction...
            var ks = new KeyStore_v2_File(store_filepath, storagepassword);
            // Add the created keys...
            var res1 = ks.AddKey_toStore(k1);
            var res2 = ks.AddKey_toStore(k2);

            // Save the store to disk...
            var saveres = ks.Save();
            if (res != 1)
            {
                // Failed to save keystore.
                return;
            }
```

## Building OGA.AppSettings.Writeable
This library is built with the new SDK-style projects.
It contains multiple projects, one for each of the following frameworks:
* NET 5
* NET 6

And, the output nuget package includes runtimes targets for:
* linux-64
* win-x64

## Framework and Runtime Support
Currently, the nuget package of this library supports the framework versions and runtimes of applications that I maintain (see above).
If someone needs others (older or newer), let me know, and I'll add them to the build script.

## Visual Studio
It is currently built using Visual Studio 2019 17.1.

## License
Please see the [License](LICENSE).
