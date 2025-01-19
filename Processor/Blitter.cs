namespace CyberedgeImageProcess2024
{
    public static class Blitter
    {
        /** dst=src */
        public const int COPY = 0;

        /** dst=255-src (8-bits and RGB) */
        public const int COPY_INVERTED = 1;

        /** Copies with white pixels transparent. */
        public const int COPY_TRANSPARENT = 2;

        /** dst=dst+src */
        public const int ADD = 3;

        /** dst=dst-src */
        public const int SUBTRACT = 4;

        /** dst=src*src */
        public const int MULTIPLY = 5;

        /** dst=dst/src */
        public const int DIVIDE = 6;

        /** dst=(dst+src)/2 */
        public const int AVERAGE = 7;

        /** dst=abs(dst-src) */
        public const int DIFFERENCE = 8;

        /** dst=dst AND src */
        public const int AND = 9;

        /** dst=dst OR src */
        public const int OR = 10;

        /** dst=dst XOR src */
        public const int XOR = 11;

        /** dst=min(dst,src) */
        public const int MIN = 12;

        /** dst=max(dst,src) */
        public const int MAX = 13;

        /** Copies with zero pixels transparent. */
        public const int COPY_ZERO_TRANSPARENT = 14;

    }
}
