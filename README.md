## **ML-Agentsとは**
**Q: ML-Agentsって何ですか？彼女ができるんですか？**

**A: ML-Agentsは、いま流行りのAIを作るための機械学習環境をUnityで構築することができる便利ツールだよ。**

**機械学習でAIを作るというと難しそうだと思うかもしれないけど、ML-Agentsでは[最低限の前提知識](https://qiita.com/intenseG/items/e394aae753a15c3e857c)さえ身につけておけば拍子抜けする程かんたんにAIを作ることができてしまうんだ。**

**さあ、みんなもML-AgentsでAI(愛)を育もう！**

## **開発環境**
当記事では以下の環境で動作を確認しました。

- Windows10

- Unity 2018.2.2f1 ※Unity 2017.4以降であればOK (2018/12/25現在)

- Anaconda3-5.3.1 + Python 3.6.7 (2018/12/25現在)

- ML-Agents v0.6.0

## **前提知識**
ML-Agentsを使い始める前に知っておくといい前提知識を以下の記事にまとめました。

[Unity ML-Agents 前提知識まとめ](https://qiita.com/intenseG/items/e394aae753a15c3e857c)

## **環境構築**
環境構築については以下の記事にまとめました。

[Unity ML-Agents 環境構築(v0.6.0対応)](https://qiita.com/intenseG/items/2e4531c711fb962d0d69)

## **囲碁ゲームを作る**
実際に動かしたい人は、このリポジトリをダウンロードしてください。

https://github.com/intenseG/GoGame

## **強化学習を使ったトレーニング**

1. Anaconda Promptを起動
2. 仮想環境を有効化
3. ハイパーパラメータを設定
4. 学習開始コマンドを実行
5. Unityでゲームスタート

### **Anaconda Promptを起動**

スタート -> すべてのプログラムの中にあるAnaconda3フォルダを開き、Anaconda Promptを起動します。

### **仮想環境を有効化**

仮想環境を有効化するには以下のコマンドを実行してください。

```
activate mlagents
```

### **ハイパーパラメータを設定**

**ml-agents/config/** にある **trainer_config.yaml** に学習を最適化するための各種パラメータを設定していきましょう。

学習用のBrainオブジェクト名に対してパラメータの値を設定すればいいのですが、パラメータ値の調整は機械学習に精通している人でも難しいとされているところです。

**とりあえず適当に以下の値を設定して後から調整していきましょう！**

**trainer_config.yaml**

```
GoGameLearningBlack:
    batch_size: 256
    buffer_size: 2048
    learning_rate: 5.0e-3
    hidden_units: 32
    num_layers: 3
    beta: 5.0e-3
    gamma: 0.9
    max_steps: 5.0e5
    summary_freq: 1000
    time_horizon: 1000

GoGameLearningWhite:
    batch_size: 256
    buffer_size: 2048
    learning_rate: 5.0e-3
    hidden_units: 32
    num_layers: 3
    beta: 5.0e-3
    gamma: 0.9
    max_steps: 5.0e5
    summary_freq: 1000
    time_horizon: 1000
```

これを**trainer_config.yaml**の末尾に追記してください。

この設定をしておかないと、学習開始コマンドを実行するときに **「そんなBrainオブジェクト存在しないよ」** とAnaconda Promptちゃんに振られてしまいます。**気を付けましょう。**

### **学習開始コマンドを実行**

ハイパーパラメータの設定が出来たところで以下のコマンドを実行します。

```
mlagents-learn config/trainer_config.yaml --run-id=GoGame --train
```

Unityのロゴが表示されたらOKです！

### **Unityでゲームスタート**

```
Start training by pressing the Play button in the Unity Editor.
```

このメッセージがAnaconda Promptに表示された後、**Unity側から再生ボタンを押す**ことで強化学習を開始することができます！

学習が始まるとAnaconda Promptに1000ステップ毎、**エージェントが得た報酬**のログが出力されます。

```Mean Reward``` の値が継続して増加していれば学習が上手くいっています(100万ステップ以上学習しないと増加しないこともあります)

途中で学習をやめたいときは、```Ctrl + C```を押すことで途中までの学習結果を**モデルファイル**として出力してくれます。

## **学習の様子**

<blockquote class="twitter-tweet" data-lang="ja"><p lang="ja" dir="ltr">ML-Agents v0.6.0で囲碁ゲーム作ってみた動画 part1<br><br>これは大体50000Stepくらい学習してる状態！ <a href="https://t.co/nIZjAfvbXy">pic.twitter.com/nIZjAfvbXy</a></p>&mdash; 欧米か@詰碁アプリ公開中！ (@oubeika11) <a href="https://twitter.com/oubeika11/status/1076451036263415808?ref_src=twsrc%5Etfw">2018年12月22日</a></blockquote>
<script async src="https://platform.twitter.com/widgets.js" charset="utf-8"></script>

<blockquote class="twitter-tweet" data-lang="ja"><p lang="ja" dir="ltr">ML-Agents v0.6.0で囲碁ゲーム作ってみた動画 part2<br><br>これは大体350000Stepくらい学習してる状態！<br>あんまり変わってないように見えるけど、TensorBoardを見る限りは少し賢くなってる！ <a href="https://t.co/75H0Lv36r3">pic.twitter.com/75H0Lv36r3</a></p>&mdash; 欧米か@詰碁アプリ公開中！ (@oubeika11) <a href="https://twitter.com/oubeika11/status/1076451504637108224?ref_src=twsrc%5Etfw">2018年12月22日</a></blockquote>
<script async src="https://platform.twitter.com/widgets.js" charset="utf-8"></script>

## **強化学習したAIと対局してみる**
それでは、強化学習によって得た**血と涙の結晶(モデルファイル)** をUnity側で設定して対局してみましょう！

まずは、Unity側でモデルファイルを入れる**Modelsフォルダ** を作ってください。

次に、**ml-agents/models/GoGame-0** の中にbytes形式のファイルが2つ生成されていることを確認しましょう。

![20181225_advent_model1.png](https://qiita-image-store.s3.amazonaws.com/0/228364/18dcaa62-21a9-e86a-c032-7203192ba22a.png)

そして、それを**Unity側のModelsフォルダ** にドラッグ＆ドロップしてインポートします(Modelsフォルダが無ければ作ってください)

ここまで出来たら、次はUnity側でBrainの設定をしていきましょう。

UnityのProjectビューのBrainフォルダの中には```GoGameLearningBlack```, ```GoGameLearningWhite```という2つのBrainがあるので、1回だけクリックして設定画面をInspectorに出してください。

Modelの箇所にBrain名と同じファイル名のbytesファイルを指定することで、```AIの頭脳``` をセットしたことになります！

![20181225_advent_brain.png](https://qiita-image-store.s3.amazonaws.com/0/228364/14cfd8fe-06d4-a7ca-d09a-7c3549c3a929.png)

**よーし！そんじゃ対局するでー！** ...と言いたいところですが、まだ設定することがあるので、もう少しだけお付き合いください＞＜

実はここまで、人間がプレイしない前提の設定で進めていたため、このまま実行すると上手く動きません。

そのため、学習用設定からプレイ用設定に切り替えるには以下の3つの設定を変更する必要があります。

- **GoAcademyのcontrolのチェックを全部外す**
- **GoAcademyのisPlayModeのチェックを入れる**

![20181225_advent_goacademy.png](https://qiita-image-store.s3.amazonaws.com/0/228364/7a88df26-acf7-15cb-bfb4-4c5bda8352a9.png)

- **GoAgent1のBrain項目にGoGamePlayer(PlayerBrain)を指定する**

![20181225_advent_goagent1.png](https://qiita-image-store.s3.amazonaws.com/0/228364/01ec086d-1801-5a7d-c077-0d967cbb8172.png)

**※現在の仕様では、人間側はGoAgent1(後攻)でプレイすることしかできません。ご了承ください。**

これでプレイ用の設定が完了しました！

**Unityでゲームを実行して、AIが初手を打ってきたら対局開始成功です！**

![20181225_advent_playing.png](https://qiita-image-store.s3.amazonaws.com/0/228364/6e27f979-53c7-c920-3cdb-2a0bcd90b3c0.png)

## **参考文献**

- [Unity ML-Agents公式リポジトリ](https://github.com/Unity-Technologies/ml-agents/tree/master/docs)
- 布留川 英一『Unityではじめる機械学習・強化学習 Unity ML-Agents実践ゲームプログラミング』（ボーンデジタル、2018）