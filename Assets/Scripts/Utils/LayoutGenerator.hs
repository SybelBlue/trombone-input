{-# LANGUAGE LambdaCase #-}

import Text.Printf
import Data.List

type Layout = (Bool, [LayoutKey])

data LayoutKey = Simple Char Size | Ambiguous [ LayoutKey ] | Alt Char Size (Maybe Char) deriving Show

data Size = Small | Medium deriving Show

sizeToBarWidth :: Size -> Int
sizeToBarWidth =
    \case
      Small -> 2
      Medium -> 3

qwerty :: Layout
qwerty = (False, map tripletIntoAmbiguous $ [ "QAZ", "WSX", "EDC", "RFV", "TGB"] ++ map reverse ["YHU", "IJN", "OKM", "PL." ])

tripletIntoAmbiguous :: String -> LayoutKey
tripletIntoAmbiguous [ a, b, c ] = Ambiguous [ Simple a Small, Simple b Medium, Simple c Small ]
tripletIntoAmbiguous _ = error "unexpected input"

layoutSize :: [LayoutKey] -> Int
layoutSize = sum . map itemSize

itemSize :: LayoutKey -> Int
itemSize (Simple _ s) = sizeToBarWidth s
itemSize (Ambiguous xs) = layoutSize xs
itemSize _ = sizeToBarWidth Small

abcde :: Layout
abcde = (False, map makeItem ['A'..'Z'])
  where makeItem x = Simple x $ if commonLetter x then Medium else Small

binnedAbcde :: Layout
binnedAbcde = (True, map Ambiguous . splitEvery 4 . map makeItem $ zip ['A'..'Z'] alt)
  where 
    makeItem (c, a) = Alt c Small a
    alt = concat . transpose $
          [ [ Just '1', Just '2', Just '3', Nothing, Nothing, Nothing, Nothing ]
          , [ Just '4', Just '5', Just '6', Nothing, Nothing, Nothing, Nothing ]
          , [ Just '7', Just '8', Just '9', Nothing, Nothing, Nothing ]
          , [ Just '*', Just '/', Just '.', Nothing, Nothing, Nothing ] 
          ]

-- https://stackoverflow.com/questions/8680888/subdividing-a-list-in-haskell
splitEvery :: Int -> [a] -> [[a]]
splitEvery _ [] = []
splitEvery n list = first : (splitEvery n rest)
  where
    (first,rest) = splitAt n list

commonLetter :: Char -> Bool
commonLetter x = elem x "ERIOTAN"

makeInitMethod :: Layout -> String
makeInitMethod (stylusMode, keys) = "\t// Auto-generated \n\tprotected override LayoutKey[] FillKeys() {\n\t\t"
  ++ "return new LayoutKey[] {\n"
  ++ (intercalate "\n" $ concat $ map (map deformat . makeConstructorLineFor stylusMode) keys)
  ++ "\n\t};\n}"


type Formatted = (Int, String, Bool)

deformat :: Formatted -> String
deformat (n, s, b) = (take n $ cycle "\t") ++ s ++ if b then "," else ""

constructorName :: Bool -> LayoutKey -> String
constructorName stylusMode (Ambiguous _) = if stylusMode then "StylusBinnedKey" else "AmbiguousKey"
constructorName stylusMode _ = if stylusMode then "StylusKey" else "SimpleKey"

makeConstructorLineFor :: Bool -> LayoutKey -> [Formatted]
makeConstructorLineFor stylusMode (Simple c s) = [(2, printf "new %s('%c', %d)" name c (sizeToBarWidth s), True)]
  where name = constructorName stylusMode $ Simple c s
makeConstructorLineFor stylusMode (Alt c s x) = [(2, printf "new %s('%c', %d%s)" name c (sizeToBarWidth s) lastArg, True)]
  where
    name = constructorName stylusMode $ Alt c s x
    lastArg = 
      case x of 
          Nothing -> ""
          Just a -> printf ", '%c'" a
makeConstructorLineFor stylusMode (Ambiguous items) = (2, "new " ++ name ++ "(true", True) : inner ++ [(2, ")", True)]
  where 
    name = constructorName stylusMode $ Ambiguous items
    inner =
      case uncons . reverse . map (\(n, s, b) -> (n + 1, s, b)) . concat $ map (makeConstructorLineFor stylusMode) items of
        Just ((n, s, _), others) ->
          reverse $ (n, s, False) : others
        Nothing ->
          []
  

main = putStr $ makeInitMethod binnedAbcde