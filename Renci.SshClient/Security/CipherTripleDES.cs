﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Renci.SshClient.Security
{
    /// <summary>
    /// Represents Triple DES encryption.
    /// </summary>
    public class CipherTripleDES : Cipher
    {
        private SymmetricAlgorithm _algorithm;

        private ICryptoTransform _encryptor;

        private ICryptoTransform _decryptor;

        /// <summary>
        /// Gets algorithm name.
        /// </summary>
        public override string Name
        {
            get { return "3des-cbc"; }
        }

        /// <summary>
        /// Gets or sets the key size, in bits, of the secret key used by the cipher.
        /// </summary>
        /// <value>
        /// The key size, in bits.
        /// </value>
        public override int KeySize
        {
            get
            {
                return this._algorithm.KeySize;
            }
        }

        /// <summary>
        /// Gets or sets the block size, in bits, of the cipher operation.
        /// </summary>
        /// <value>
        /// The block size, in bits.
        /// </value>
        public override int BlockSize
        {
            get
            {
                return this._algorithm.BlockSize / 8;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CipherTripleDES"/> class.
        /// </summary>
        public CipherTripleDES()
        {
            this._algorithm = new System.Security.Cryptography.TripleDESCryptoServiceProvider();
            this._algorithm.Mode = System.Security.Cryptography.CipherMode.CBC;
            this._algorithm.Padding = System.Security.Cryptography.PaddingMode.None;
        }

        /// <summary>
        /// Encrypts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>
        /// Encrypted data
        /// </returns>
        public override IEnumerable<byte> Encrypt(IEnumerable<byte> data)
        {
            if (this._encryptor == null)
            {
                this._encryptor = this._algorithm.CreateEncryptor(this.Key.Take(this.KeySize / 8).ToArray(), this.Vector.Take(this.BlockSize).ToArray());
            }

            var input = data.ToArray();
            var output = new byte[input.Length];
            var writtenBytes = this._encryptor.TransformBlock(input, 0, input.Length, output, 0);

            if (writtenBytes < input.Length)
            {
                throw new InvalidOperationException("Encryption error.");
            }

            return output;
        }

        /// <summary>
        /// Decrypts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>
        /// Decrypted data
        /// </returns>
        public override IEnumerable<byte> Decrypt(IEnumerable<byte> data)
        {
            if (this._decryptor == null)
            {
                this._decryptor = this._algorithm.CreateDecryptor(this.Key.Take(this.KeySize / 8).ToArray(), this.Vector.Take(this.BlockSize).ToArray());
            }

            var input = data.ToArray();
            var output = new byte[input.Length];
            var writtenBytes = this._decryptor.TransformBlock(input, 0, input.Length, output, 0);

            if (writtenBytes < input.Length)
            {
                throw new InvalidOperationException("Encryption error.");
            }

            return output;
        }

        private bool _isDisposed = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._isDisposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (this._algorithm != null)
                    {
                        this._algorithm.Dispose();
                        this._algorithm = null;
                    }
                }

                // Note disposing has been done.
                _isDisposed = true;
            }
        }
    }
}
