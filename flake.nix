{
  description = ".NET project template";

  inputs = {
    nixpkgs.url = "github:nixos/nixpkgs/nixos-unstable";
    utils.url = "github:numtide/flake-utils";
  };

  outputs = inputs@{ nixpkgs, ... }:
    inputs.utils.lib.eachDefaultSystem (system:
      let
        pkgs = import nixpkgs { inherit system; };
      in
      rec {
        # `nix develop`
        devShells.default = with pkgs; mkShell {
          buildInputs = [
            docker
            dotnet-sdk_6
            nodejs
            gnumake
            icu
            openssl
            sqlite
          ];

          shellHook = ''
            export DOTNET_ROOT=${dotnet-sdk_6}
            export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:${lib.makeLibraryPath [ icu openssl ]}
          '';
        };
      });
}
