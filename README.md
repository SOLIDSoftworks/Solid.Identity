# Solid.Identity [![build](https://github.com/SOLIDSoftworks/Solid.Identity/actions/workflows/test.yml/badge.svg?branch=main)](https://github.com/SOLIDSoftworks/Solid.Identity/actions/workflows/test.yml)
This repository is a collection of packages that we have written over the years, consolodated into a single repository.

## Documentation

The website source is in [`docs/`](docs/index.md), with navigation in [`mkdocs.yml`](mkdocs.yml) and a [Read the Docs configuration](.readthedocs.yaml). To build it locally, install Python and run:

```console
python -m pip install -r docs/requirements.txt
python -m mkdocs build --strict
```

Run `python -m mkdocs serve` to preview the site locally. Read the Docs can build the site directly from this repository using `.readthedocs.yaml`.

## The packages
- Solid.Http.Core
- Solid.Http.Json
- Solid.Http.Xml
- Solid.Http.Zip
- Solid.Http
- Solid.IdentityModel.Tokens
- Solid.IdentityModel.Tokens.Saml
- Solid.IdentityModel.Xml
- Solid.IdentityModel.Protocols.WsTrust
- Solid.Identity.Protocols.Saml2p
- Solid.Identity.Protocols.WsTrust
- Solid.ServiceModel
- Solid.Extensions.ServiceModel
- Solid.ServiceModel.Security.WsTrust
- Solid.Testing.Core
- Solid.Testing.AspNetCore
- Solid.Testing.AspNetCore.Extensions.Https
- Solid.Testing.AspNetCore.Extensions.XUnit
- Solid.Extensions.AspNetCore.Soap
- Solid.Extensions.AspNetCore.XUnit.Soap
- Solid.Testing.Certificates
